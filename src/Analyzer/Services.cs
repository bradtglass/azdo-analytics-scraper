using System.Linq;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping;
using Autofac;
using Microsoft.EntityFrameworkCore;

namespace Analyzer;

public static class Services
{
    private static AnalyticsScraperClientOptions CreateClientOptions()
        => new(Configuration.GetOrganisation(),
            Configuration.GetAccessToken(),
            Configuration.GetPolicy());

    private static DbContextOptions<DevOpsContext> CreateContextOptions()
    {
        DbContextOptionsBuilder<DevOpsContext> builder = new();
        builder.UseSqlServer(Configuration.GetConnectionString());
#if DEBUG
        builder.EnableSensitiveDataLogging();
#endif

        return builder.Options;
    }

    public static IContainer CreateContainer()
    {
        var builder = new ContainerBuilder();

        // API Client
        builder.Register(_ => CreateClientOptions())
            .As<AnalyticsScraperClientOptions>()
            .SingleInstance();

        builder.RegisterType<AnalyticsScraperClient>()
            .AsSelf()
            .InstancePerLifetimeScope();
        
        // Database Context
        builder.Register(_ => CreateContextOptions())
            .As<DbContextOptions>()
            .SingleInstance();

        builder.RegisterType<DevOpsContext>()
            .AsSelf()
            .InstancePerDependency();
        
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
            .Where(t => t.GetInterfaces().Any(i=>i.IsConstructedGenericType && i.GetGenericTypeDefinition()==typeof(IScraper<>)))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        return builder.Build();
    }
}