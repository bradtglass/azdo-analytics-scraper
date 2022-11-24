using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

public class DevOpsContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DevOpsGuid>()
            .HaveConversion<DevOpsGuid.EfCoreValueConverter>();
        builder.Properties<DevOpsIntId>()
            .HaveConversion<DevOpsIntId.EfCoreValueConverter>();
    }

    public DevOpsContext(DbContextOptions options) : base(options) { }

    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Identity> Identities => Set<Identity>();
    public DbSet<Push> Pushes => Set<Push>();
    public DbSet<PullRequest> PullRequests => Set<PullRequest>();
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Project> Projects => Set<Project>();
}