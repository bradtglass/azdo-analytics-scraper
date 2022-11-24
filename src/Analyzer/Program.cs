using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Analyzer;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        const string dateRaw = "2022-11-22";

        using var scraperClient = CreateClient();
        await using var context = CreateContext();

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
    
    private static AnalyticsScraperClient CreateClient()
    =>new(Configuration.GetOrganisation(),
        Configuration.GetAccessToken(),
        Configuration.GetPolicy());

    private static DevOpsContext CreateContext()
    {
        DbContextOptionsBuilder<DevOpsContext> builder = new();
        builder.UseSqlServer(Configuration.GetConnectionString());
#if DEBUG
        builder.EnableSensitiveDataLogging();
#endif

        return new DevOpsContext(builder.Options);
    }
}