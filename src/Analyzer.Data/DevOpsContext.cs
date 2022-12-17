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
        builder.Properties<GitSha>()
            .HaveConversion<GitSha.EfCoreValueConverter>()
            .HaveMaxLength(40)
            .AreFixedLength();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("azdo");

        modelBuilder.Entity<Commit>()
            .HasOne(c => c.Author)
            .WithMany(i => i.AuthoredCommits)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Commit>()
            .HasOne(c => c.Commiter)
            .WithMany(i => i.CommitedCommits)
            .HasForeignKey(c => c.CommiterId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PullRequest>()
            .HasOne(pr => pr.MergeCommit)
            .WithOne(c => c.MergingPullRequest)
            .HasForeignKey<PullRequest>(pr => pr.MergeCommitId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Push>()
            .HasOne(p => p.Identity)
            .WithMany(i => i.Pushes)
            .HasForeignKey(p => p.IdentityId);
        modelBuilder.Entity<PullRequest>()
            .HasOne(p => p.CreatedBy)
            .WithMany(i => i.PullRequests)
            .HasForeignKey(p => p.CreatedById);
    }
}