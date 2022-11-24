using Analyzer.Client;
using Analyzer.Data;
using Microsoft.EntityFrameworkCore;

namespace Analyzer;

public static class Services
{
    public static AnalyticsScraperClient CreateClient()
        => new(Configuration.GetOrganisation(),
            Configuration.GetAccessToken(),
            Configuration.GetPolicy());

    public static DevOpsContext CreateContext()
    {
        DbContextOptionsBuilder<DevOpsContext> builder = new();
        builder.UseSqlServer(Configuration.GetConnectionString());
#if DEBUG
        builder.EnableSensitiveDataLogging();
#endif

        return new DevOpsContext(builder.Options);
    }
}