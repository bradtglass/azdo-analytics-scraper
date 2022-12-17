using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Serilog;

namespace Analyzer.Scraping.PullRequests;

public class PullRequestScraper : IScraper<PullRequestScraperDefinition>
{
    private readonly AnalyticsScraperClient client;
    private readonly DevOpsContext context;
    private readonly IIdentityCache identityCache;

    public PullRequestScraper(DevOpsContext context, AnalyticsScraperClient client, IIdentityCache identityCache)
    {
        this.context = context;
        this.client = client;
        this.identityCache = identityCache;
    }

    public async IAsyncEnumerable<IScraperDefinition> ScrapeAsync(PullRequestScraperDefinition definition,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await ScrapeForUntrackedCompleted(definition.ProjectId, definition.RepoId, ct);
        yield break;
    }

    private async ValueTask ScrapeForUntrackedCompleted(DevOpsGuid projectId, DevOpsGuid repoId,
        CancellationToken ct)
    {
        var devOpsIdentity = await context.FindDevOpsIdentityAsync();

        if (devOpsIdentity is null)
        {
            Log.Error("Cannot find TFS user identity - cannot scrape for completed PRs");
            return;
        }

        var mergeCommitShas = await context.Commits.Where(c => c.MergingPullRequest == null)
            .Where(c => c.Author == devOpsIdentity)
            .Where(c => c.Push.Repository.DevOpsId == repoId)
            .Select(c => c.Sha)
            .ToListAsync(ct);

        var pullRequests = await client.GetPullRequestsFromMergeCommitsAsync(projectId.Value,
            repoId.Value,
            mergeCommitShas.Select(c=>c.Value));

        await InsertBatchAsync(repoId, pullRequests, ct);
    }

    private async Task InsertBatchAsync(DevOpsGuid repoDevOpsId, IEnumerable<GitPullRequest> scrapedPullRequests,
        CancellationToken ct)
    {
        var repo = await context.Repositories.SingleAsync(p => p.DevOpsId == repoDevOpsId, ct);

        foreach (var pr in scrapedPullRequests) await InsertAsync(repo, pr);

        await context.SaveChangesAsync(ct);
    }

    private async ValueTask InsertAsync(Repository repository, GitPullRequest pr)
    {
        var state = MapState(pr);
        var closedTimestamp = state is PullRequestState.Active or PullRequestState.Draft
            ? null
            : (DateTimeOffset?)new DateTimeOffset(pr.ClosedDate, TimeSpan.Zero);
        var mergeCommitId = state == PullRequestState.Completed
            ? pr.LastMergeCommit.CommitId
            : null;

        var createdBy = await identityCache.GetIdentityIdAsync(pr.CreatedBy);

        var entity = new PullRequest(pr.Title,
            new DateTimeOffset(pr.CreationDate, TimeSpan.Zero),
            closedTimestamp,
            state,
            DevOpsIntId.From(pr.PullRequestId))
        {
            Repository = repository,
            CreatedById = createdBy,
            MergeCommitId = GitSha.From(mergeCommitId)
        };

        context.Add(entity);
    }

    private static PullRequestState MapState(GitPullRequest pr)
    {
        if (pr.IsDraft ?? false)
            return PullRequestState.Draft;

        return pr.Status switch
        {
            PullRequestStatus.NotSet => throw new ArgumentException("Status must be set", nameof(pr)),
            PullRequestStatus.Active => PullRequestState.Active,
            PullRequestStatus.Abandoned => PullRequestState.Abandoned,
            PullRequestStatus.Completed => PullRequestState.Completed,
            PullRequestStatus.All => throw new ArgumentException("Status cannot be all", nameof(pr)),
            _ => throw new ArgumentOutOfRangeException(nameof(pr), pr.Status, "Unknown PR status")
        };
    }
}