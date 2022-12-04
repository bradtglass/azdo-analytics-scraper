using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping;
using Analyzer.Scraping.Projects;
using Autofac;
using Microsoft.EntityFrameworkCore;

namespace Analyzer;

internal static class Program
{
    private const string dateRaw = "2022-11-22";

    public static async Task Main(string[] args)
    {
        await using var container = Services.CreateContainer();

        await using (var scope = container.BeginLifetimeScope())
        {
            await EnsureMigrationsAsync(scope);
        }

        await using (var scope = container.BeginLifetimeScope())
        {
            await ScrapeAsync(scope);
        }
    }

    private static async ValueTask EnsureMigrationsAsync(IComponentContext scope)
    {
        var context = scope.Resolve<DevOpsContext>();
        await context.Database.MigrateAsync();
    }

    private static async ValueTask ScrapeAsync(ILifetimeScope scope)
    {
        var date = DateTimeOffset.Parse(dateRaw, styles: DateTimeStyles.AssumeLocal);
        var dates = new DateRange(date, date.AddDays(1));

        ConcurrentQueue<IScraperDefinition> definitions = new();
        definitions.Enqueue(new ProjectScraperDefinition(dates));

        while (definitions.TryDequeue(out var definition))
        {
            await using var innerScope = scope.BeginLifetimeScope();

            var runner = innerScope.Resolve<IScraperRunner>();
            await foreach (var next in runner.RunAsync(definition, default)) definitions.Enqueue(next);
        }
    }
}