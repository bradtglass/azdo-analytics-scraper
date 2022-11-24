using Microsoft.EntityFrameworkCore;

namespace Data;

public class DevOpsContext : DbContext
{
    public DevOpsContext(DbContextOptions options) : base(options) { }

    public DbSet<Commit> Commits => Set<Commit>();
    public DbSet<Identity> Identities => Set<Identity>();
    public DbSet<Push> Pushes => Set<Push>();
    public DbSet<PullRequest> PullRequests => Set<PullRequest>();
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Project> Projects => Set<Project>();
}