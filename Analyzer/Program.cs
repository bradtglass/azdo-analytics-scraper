
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Analyzer.Client;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Polly;

namespace Analyzer;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        const string dateRaw = "2022-10-07";
        string organisation = Environment.GetEnvironmentVariable("AZ_DO_ORG") ?? throw new Exception("Azure DevOps organisation not set");
        string collectionUri = $"https://dev.azure.com/{organisation}";
        string pat = Environment.GetEnvironmentVariable("AZ_DO_PAT") ?? throw new Exception("Azure DevOps PAT not set");

        var retryPolicy = Policy<HttpResponseMessage>.HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3,
                (_, result, _) =>
                {
                    var pause = result.Result.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
                    return pause;
                }, (_, _, _, _) => Task.CompletedTask);
        
        using var scraperClient = new AnalyticsScraperClient(organisation,
            pat,
            retryPolicy);

        var projects2 = await scraperClient.GetProjectsAsync().ToListAsync();

        var date = DateTime.Parse(dateRaw);
        
        var cred = new VssBasicCredential(string.Empty, pat);
        var connection = new VssConnection(new Uri(collectionUri), cred);

        using var projectHttpClient =await connection.GetClientAsync<ProjectHttpClient>();
        using var buildClient =await connection.GetClientAsync<BuildHttpClient>();
        using var gitClient =await connection.GetClientAsync<GitHttpClient>();
        
        
        var projects = await projectHttpClient.GetProjects(ProjectState.All);
        // List<Build> builds = new();
        // foreach (var project in projects)
        // {
        //     var projectBuilds = await buildClient.GetBuildsAsync(project.Id,minFinishTime:date, maxFinishTime:date+TimeSpan.FromDays(1));
        //     builds.AddRange(projectBuilds);
        // }
        //
        // Console.WriteLine($"{builds.Count} builds found on {date:d}");
        // foreach (var stateGroup in builds.Where(b=>b.Status == BuildStatus.Completed).GroupBy(b=>b.Result))
        // {
        //     Console.WriteLine($"{stateGroup.Key}:");
        //     foreach (var build in stateGroup)
        //     {
        //         Console.Write('\t');
        //         Console.WriteLine($"{build.Definition.Name} - {build.BuildNumber}");
        //     }
        // }
        
        Console.WriteLine();
        Console.WriteLine();

        //Project a4d19e95-908f-4fe6-824d-ff392eb20f09
        //Repo bca5fb4e-6e2b-42e0-ad76-f1597633469c
        List<GitCommitRef> commits = new();
        var criteria = new GitQueryCommitsCriteria
        {
            FromDate = DateTime.MinValue.ToString("O"),
            ToDate = date.AddDays(1).ToString("O"),
        };
        foreach (var project in projects)
        {
            foreach (var repo in await gitClient.GetRepositoriesAsync(project.Id))
            {
                try
                {
                    var theseCommits = await gitClient.GetCommitsBatchAsync(criteria, project.Id, repo.Id);
                    commits.AddRange(theseCommits);

                    if (theseCommits.Count == 100)
                    {
                        var paginator = new TokenlessPaginator<GitCommitRef>(async (top, skip) => await gitClient.GetCommitsBatchAsync(criteria,project.Id,repo.Id,top:top, skip:skip));
                        var list = await paginator.GetAsync().ToListAsync();
                        Console.WriteLine($"original count: {theseCommits.Count}");
                        Console.WriteLine($"paginated count: {list.Count}");
                        
                        return;
                    }
                }
                catch (VssServiceException e)
                {
                }
            }
        }

        var authors = commits.GroupBy(c=>c.Author.Email)
            .Select(g=>commits.First(c=>c.Committer.Email==g.Key).Author)
            .ToList();

        Console.WriteLine($"There were {commits.Count} on {date:d}!");
        Console.WriteLine($"{authors.Count} authors got involved:");
        foreach (var author in authors)
        {
            Console.Write('\t');
            Console.WriteLine($"{author.Name} - {author.Email}");
        }
    }
}

