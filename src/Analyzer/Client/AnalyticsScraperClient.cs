using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Flurl;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Polly;

namespace Analyzer.Client;

public sealed class AnalyticsScraperClient : IDisposable
{
    private readonly HttpClient client;
    private readonly MediaTypeFormatter[] mediaFormatters = { new VssJsonMediaTypeFormatter() };
    private readonly string organisation;

    public AnalyticsScraperClient(string organisation, string pat, AsyncPolicy<HttpResponseMessage> policy)
    {
        this.organisation = organisation;
        client = HttpClientFactory.Create(ApiVersioningHandler.Instance,
            new DevOpsAuthenticatingHandler(pat),
            new PollyHandler(policy));
    }

    public void Dispose()
    {
        client.Dispose();
    }

    private Url GetBaseGitAddress(Guid projectId, Guid repositoryId) =>
        $"https://dev.azure.com/{organisation}/{projectId}/_apis/git/repositories/{repositoryId}";

    private Url GetBaseGitAddress(Guid projectId) => $"https://dev.azure.com/{organisation}/{projectId}/_apis/git";

    private Url GetBaseAddress() => $"https://dev.azure.com/{organisation}/_apis";

    private async ValueTask<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<T>(mediaFormatters);
    }

    private async ValueTask<IContinuablePage<T>> ReadJsonPageAsync<T>(HttpResponseMessage response)
    {
        var items = await ReadJsonAsync<VssJsonCollectionWrapper<List<T>>>(response);
        var continuationToken = response.Headers.TryGetValues("x-ms-continuationtoken", out var values)
            ? values.FirstOrDefault()
            : null;

        return new ContinuablePage<T>(continuationToken, items.Value);
    }

    private async ValueTask<IContinuablePage<TeamProject>> GetProjectsAsync(string? continuationToken)
    {
        using var request = GetBaseAddress()
            .AppendPathSegment("projects")
            .SetQueryParam("continuationToken", continuationToken)
            .Get();

        using var response = await client.SendAsync(request);
        return await ReadJsonPageAsync<TeamProject>(response);
    }

    private async ValueTask<IEnumerable<GitPush>> GetPushesAsync(Guid projectId, Guid repoId, PageIndex page,
        DateRange dates)
    {
        using var request = GetBaseGitAddress(projectId, repoId)
            .AppendPathSegment("pushes")
            .SetPage(page)
            .SetDates(dates)
            .Get();

        using var response = await client.SendAsync(request);
        return await ReadJsonPageAsync<GitPush>(response);
    }

    public IAsyncEnumerable<TeamProject> GetProjectsAsync() =>
        new TokenPaginator<TeamProject>(GetProjectsAsync).GetAsync();

    public async IAsyncEnumerable<GitRepository> GetEnabledRepositoriesAsync(Guid projectId)
    {
        using var request = GetBaseGitAddress(projectId)
            .AppendPathSegment("repositories")
            .Get();

        using var response = await client.SendAsync(request);
        var repos = await ReadJsonPageAsync<GitRepository>(response);

        foreach (var repo in repos.Where(r => !r.IsDisabled.GetValueOrDefault(false)))
            yield return repo;
    }

    public IAsyncEnumerable<GitPush> GetPushesAsync(Guid projectId, Guid repoId, DateRange dates)
    {
        return new TokenlessPaginator<GitPush>(page => GetPushesAsync(projectId, repoId, page, dates))
            .GetAsync();
    }
}