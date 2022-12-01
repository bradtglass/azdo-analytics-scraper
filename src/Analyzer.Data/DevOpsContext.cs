using Microsoft.EntityFrameworkCore;

namespace Analyzer.Data;

public class DevOpsContext : DbContext
{
    public DevOpsContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Identity> Identities => Set<Identity>();
    public DbSet<Push> Pushes => Set<Push>();
    public DbSet<PullRequest> PullRequests => Set<PullRequest>();
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Project> Projects => Set<Project>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DevOpsGuid>()
            .HaveConversion<DevOpsGuid.EfCoreValueConverter>();
        builder.Properties<DevOpsIntId>()
            .HaveConversion<DevOpsIntId.EfCoreValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("azdo");
        
        modelBuilder.Entity<Commit>()
            .HasOne(c => c.Author)
            .WithMany(i => i.AuthoredCommits)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Commit>()
            .HasOne(c => c.Commiter)
            .WithMany(i => i.CommitedCommits)
            .OnDelete(DeleteBehavior.Restrict);
    }
}