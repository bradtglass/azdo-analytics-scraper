using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping.PullRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Analyzer.Scraping.Pushes;

public class PushScraper : IScraper<PushScraperDefinition>
{
    private readonly AnalyticsScraperClient client;
    private readonly DevOpsContext context;
    private readonly IIdentityCache identityCache;

    public PushScraper(DevOpsContext context, AnalyticsScraperClient client, IIdentityCache identityCache)
    {
        this.context = context;
        this.client = client;
        this.identityCache = identityCache;
    }

    public async IAsyncEnumerable<IScraperDefinition> ScrapeAsync(PushScraperDefinition definition,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var pushEnumerable = client.GetPushesAsync(definition.ProjectId.Value,
            definition.RepoId.Value,
            definition.Window);

        await foreach (var pushes in pushEnumerable.ChunkAsync(20, ct))
            await ScrapeBatchAsync(definition.RepoId, pushes, ct);

        yield return new PullRequestScraperDefinition(definition.ProjectId, definition.RepoId);
    }

    private async Task ScrapeBatchAsync(DevOpsGuid repoDevOpsId, IReadOnlyCollection<GitPush> scrapedPushes,
        CancellationToken ct)
    {
        var repo = await context.Repositories.SingleAsync(p => p.DevOpsId == repoDevOpsId, ct);

        var ids = scrapedPushes.Select(p => DevOpsIntId.From(p.PushId)).ToList();

        var existing = await context.Pushes
            .Where(p => p.Repository == repo)
            .Where(p => ids.Contains(p.DevOpsId))
            .ToListAsync(ct);

        foreach (var push in scrapedPushes)
        {
            if (existing.Any(e => e.DevOpsId.Value == push.PushId))
                continue;

            var devOpsId = DevOpsIntId.From(push.PushId);
            await UpdateAsync(repo, push, existing.FirstOrDefault(r => r.DevOpsId == devOpsId));
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task UpdateAsync(Repository repository, GitPush scraped, Push? entity)
    {
        entity ??= new Push(DevOpsIntId.From(scraped.PushId));

        entity.Repository = repository;
        entity.Timestamp = new DateTimeOffset(scraped.Date, TimeSpan.Zero);
        entity.IdentityId = await identityCache.GetIdentityIdAsync(scraped.PushedBy);
        entity.Commits.AddRange(await ProcessCommitsAsync(scraped).ToListAsync());

        context.AddIfUntracked(entity);
    }

    private IAsyncEnumerable<Commit> ProcessCommitsAsync(GitPush push)
    {
        return client.GetPushCommitsAsync(push.Repository.ProjectReference.Id, push.Repository.Id, push.PushId)
            .SelectAwait(async gitCommit => new Commit(GitSha.From(gitCommit.CommitId),
                new DateTimeOffset(gitCommit.Committer.Date, TimeSpan.Zero),
                new DateTimeOffset(gitCommit.Author.Date, TimeSpan.Zero),
                gitCommit.Comment)
            {
                AuthorId = await identityCache.GetIdentityIdAsync(gitCommit.Author),
                CommiterId = await identityCache.GetIdentityIdAsync(gitCommit.Committer)
            });
    }
}