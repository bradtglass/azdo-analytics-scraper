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
using Microsoft.VisualStudio.Services.WebApi;

namespace Analyzer.Scraping.Pushes;

public class PushScraper : IScraper<PushScraperDefinition>
{
    private readonly Dictionary<string, Identity> cachedIdentities = new();
    private readonly AnalyticsScraperClient client;
    private readonly DevOpsContext context;

    public PushScraper(DevOpsContext context, AnalyticsScraperClient client)
    {
        this.context = context;
        this.client = client;
    }

    public async IAsyncEnumerable<IScraperDefinition> ScrapeAsync(PushScraperDefinition definition,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var pushEnumerable = client.GetPushesAsync(definition.ProjectId.Value,
            definition.RepoId.Value,
            definition.Window);

        await foreach (var pushes in pushEnumerable.ChunkAsync(20, ct))
        {
            await ScrapeBatchAsync(definition.RepoId, pushes, ct);
        }

        yield break;
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
        entity.Identity = await GetOrAddIdentityAsync(scraped.PushedBy);
        entity.Commits.AddRange(await ProcessCommitsAsync(scraped).ToListAsync());

        context.AddIfUntracked(entity);
    }

    private IAsyncEnumerable<Commit> ProcessCommitsAsync(GitPush push)
    {
        return client.GetPushCommitsAsync(push.Repository.ProjectReference.Id, push.Repository.Id, push.PushId)
            .SelectAwait(async gitCommit => new Commit(Commit.GetSha(gitCommit.CommitId),
                new DateTimeOffset(gitCommit.Committer.Date, TimeSpan.Zero),
                new DateTimeOffset(gitCommit.Author.Date, TimeSpan.Zero),
                gitCommit.Comment)
            {
                Author = await GetOrAddIdentityAsync(gitCommit.Author),
                Commiter = await GetOrAddIdentityAsync(gitCommit.Committer)
            });
    }

    private async ValueTask<Identity> GetOrAddIdentityAsync(GitUserDate gitUserDate) =>
        await GetOrAddIdentityAsync(gitUserDate.Name, gitUserDate.Email, null);

    private async ValueTask<Identity> GetOrAddIdentityAsync(IdentityRef idRef)
        => await GetOrAddIdentityAsync(idRef.DisplayName, idRef.UniqueName, DevOpsGuid.From(Guid.Parse(idRef.Id)));

    private async ValueTask<Identity> GetOrAddIdentityAsync(string name, string email, DevOpsGuid? devOpsId)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        if (cachedIdentities.TryGetValue(email, out var identity)) return identity;

        identity = await context.Identities.SingleOrDefaultAsync(i => i.UniqueName == email);

        if (identity is null)
        {
            if (!devOpsId.HasValue)
            {
                var id = await client.FindIdentityByEmailAsync(email);
                if (id is not null)
                    devOpsId = DevOpsGuid.From(id.Id);
            }

            identity = new Identity
            {
                DisplayName = name,
                UniqueName = email,
                DevOpsId = devOpsId
            };

            context.Identities.Add(identity);
        }

        cachedIdentities.Add(email, identity);
        return identity;
    }
}