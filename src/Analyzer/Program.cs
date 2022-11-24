using System;
using System.Globalization;
using System.Threading.Tasks;
using Analyzer.Client;

namespace Analyzer;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        const string dateRaw = "2022-11-22";

        using var scraperClient = Services.CreateClient();
        await using var context = Services.CreateContext();

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