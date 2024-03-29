﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping.Pushes;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Analyzer.Scraping.Repos;

public class RepoScraper : IScraper<RepoScraperDefinition>
{
    private readonly AnalyticsScraperClient client;
    private readonly DevOpsContext context;

    public RepoScraper(DevOpsContext context, AnalyticsScraperClient client)
    {
        this.context = context;
        this.client = client;
    }

    public async IAsyncEnumerable<IScraperDefinition> ScrapeAsync(RepoScraperDefinition definition,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var reposEnumerable = client.GetEnabledRepositoriesAsync(definition.ProjectId.Value);

        await foreach (var repos in reposEnumerable.ChunkAsync(20, ct))
        {
            await ScrapeBatchAsync(definition.ProjectId, repos, ct);

            foreach (var repo in repos)
                yield return new PushScraperDefinition(definition.Window,
                    definition.ProjectId,
                    DevOpsGuid.From(repo.Id));
        }
    }

    private async Task ScrapeBatchAsync(DevOpsGuid projectDevOpsId, IReadOnlyCollection<GitRepository> scrapedRepos,
        CancellationToken ct)
    {
        var project = await context.Projects.SingleAsync(p => p.DevOpsId == projectDevOpsId, ct);

        var ids = scrapedRepos.Select(r => DevOpsGuid.From(r.Id)).ToList();

        var existing = await context.Repositories
            .Where(r => ids.Contains(r.DevOpsId))
            .ToListAsync(ct);

        foreach (var repo in scrapedRepos)
        {
            var devOpsId = DevOpsGuid.From(repo.Id);
            UpdateOrInsert(project, repo, existing.FirstOrDefault(r => r.DevOpsId == devOpsId));
        }

        await context.SaveChangesAsync(ct);
    }

    private void UpdateOrInsert(Project project, GitRepository scraped, Repository? entity)
    {
        entity ??= new Repository(DevOpsGuid.From(scraped.Id));

        entity.Project = project;
        entity.Name = scraped.Name;

        context.AddIfUntracked(entity);
    }
}