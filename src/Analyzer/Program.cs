﻿using System.Globalization;
using System.Net;
using Analyzer.Client;
using Polly;

namespace Analyzer;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        const string dateRaw = "2022-11-22";
        var organisation = Environment.GetEnvironmentVariable("AZ_DO_ORG") ??
                           throw new Exception("Azure DevOps organisation not set");
        var pat = Environment.GetEnvironmentVariable("AZ_DO_PAT") ?? throw new Exception("Azure DevOps PAT not set");

        var retryPolicy = Policy<HttpResponseMessage>.HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3,
                (_, result, _) =>
                {
                    var pause = result.Result.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
                    return pause;
                }, (_, _, _, _) => Task.CompletedTask);

        using var scraperClient = new AnalyticsScraperClient(organisation,
            pat,
            retryPolicy);

        var date = DateTimeOffset.Parse(dateRaw, styles: DateTimeStyles.AssumeLocal);
        var dates = new DateRange(date, date.AddDays(1));
        await foreach (var project in scraperClient.GetProjectsAsync())
        {
            Console.WriteLine(project.Name);
            await foreach (var repo in scraperClient.GetEnabledRepositoriesAsync(project.Id))
            {
                Console.WriteLine($"- {repo.Name}");
                await foreach (var push in scraperClient.GetPushesAsync(project.Id, repo.Id, dates))
                    Console.WriteLine($"-- {push.PushedBy.DisplayName}");
            }
        }
    }
}