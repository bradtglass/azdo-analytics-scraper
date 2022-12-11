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
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Analyzer;

internal static class Program
{
    private const string fromDateRaw = "2022-11-01";
    private const string toDateRaw = "2022-11-30";

    public static async Task Main(string[] args)
    {
        try
        {
            ConfigureLogging();
        
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
        catch (Exception e)
        {
            Log.Fatal(e, "An error was caught at the application root");
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();
        
        Log.Information("Logging configured");
    }

    private static async ValueTask EnsureMigrationsAsync(IComponentContext scope)
    {
        var context = scope.Resolve<DevOpsContext>();
        Log.Information("Applying any outstanding migrations");
        await context.Database.MigrateAsync();
    }

    private static async ValueTask ScrapeAsync(ILifetimeScope scope)
    {
        var fromDate = DateTimeOffset.Parse(fromDateRaw, styles: DateTimeStyles.AssumeLocal);
        var toDate = DateTimeOffset.Parse(toDateRaw, styles: DateTimeStyles.AssumeLocal);
        var dates = new DateRange(fromDate, toDate);

        ConcurrentQueue<IScraperDefinition> definitions = new();
        definitions.Enqueue(new ProjectScraperDefinition(dates));

        Log.Information("Beginning scraping");
        while (definitions.TryDequeue(out var definition))
        {
            await using var innerScope = scope.BeginLifetimeScope();
            
            Log.Debug("Running data scraping for {@Definition}", definition);
            var runner = innerScope.Resolve<IScraperRunner>();
            await foreach (var next in runner.RunAsync(definition, default)) definitions.Enqueue(next);
            
            Log.Debug("{Count} remaining definitions to scrape", definitions.Count);
        }
    }
}