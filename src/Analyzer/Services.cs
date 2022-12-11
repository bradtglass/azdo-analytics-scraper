using System.Linq;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Analyzer;

public static class Services
{
    private static AnalyticsScraperClientOptions CreateClientOptions()
        => new(Configuration.GetOrganisation(),
            Configuration.GetAccessToken(),
            Configuration.GetPolicy());

    private static DbContextOptions<DevOpsContext> CreateContextOptions(ILoggerFactory loggerFactory)
    {
        DbContextOptionsBuilder<DevOpsContext> builder = new();
        builder.UseSqlServer(Configuration.GetConnectionString())
            .UseLoggerFactory(loggerFactory);

#if DEBUG
        builder.EnableSensitiveDataLogging();
#endif

        return builder.Options;
    }

    public static IContainer CreateContainer()
    {
        var builder = new ContainerBuilder();

        // Logging
        builder.Register(_ => LoggerFactory.Create(b => b.AddSerilog()))
            .As<ILoggerFactory>()
            .SingleInstance();

        // API Client
        builder.Register(_ => CreateClientOptions())
            .As<AnalyticsScraperClientOptions>()
            .SingleInstance();

        builder.RegisterType<AnalyticsScraperClient>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Database Context
        builder.Register(cc => CreateContextOptions(cc.Resolve<ILoggerFactory>()))
            .As<DbContextOptions>()
            .SingleInstance();

        builder.RegisterType<DevOpsContext>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<IdentityCache>()
            .As<IIdentityCache>()
            .SingleInstance();

        // Scraper Running
        builder.RegisterType<ScraperRunner>()
            .As<IScraperRunner>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ScraperDefinitionSerializer>()
            .As<IScraperDefinitionInterpreter>()
            .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(GenericScraperRunner<>))
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(typeof(Services).Assembly)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IScraper<>)))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        Log.Information("Building services container");
        return builder.Build();
    }
}