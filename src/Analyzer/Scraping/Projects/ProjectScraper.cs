using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Analyzer.Client;
using Analyzer.Data;
using Analyzer.Scraping.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Analyzer.Scraping.Projects;

public class ProjectScraper : IScraper<ProjectScraperDefinition>
{
    private readonly AnalyticsScraperClient client;
    private readonly DevOpsContext context;

    public ProjectScraper(DevOpsContext context, AnalyticsScraperClient client)
    {
        this.context = context;
        this.client = client;
    }

    public async IAsyncEnumerable<IScraperDefinition> ScrapeAsync(ProjectScraperDefinition definition,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var project in client.GetProjectsAsync().WithCancellation(ct))
        {
            ct.ThrowIfCancellationRequested();
            await Upsert(project, ct);
            yield return GenerateChild(definition, project);
        }
    }

    private static IScraperDefinition GenerateChild(ProjectScraperDefinition definition, TeamProject project)
        => new RepoScraperDefinition(definition.Window, DevOpsGuid.From(project.Id));

    private async ValueTask Upsert(TeamProject scrapedProject, CancellationToken ct)
    {
        var devOpsId = DevOpsGuid.From(scrapedProject.Id);
        var project = await context.Projects.FirstOrDefaultAsync(p => p.DevOpsId == devOpsId, ct);
        project ??= new Project(devOpsId);

        project.Organisation = client.Organisation;
        project.Name = scrapedProject.Name;

        context.AddIfUntracked(project);
        await context.SaveChangesAsync(ct);
    }
}