using System.Net.Http.Formatting;
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
        $" https://dev.azure.com/{organisation}/{projectId}/_apis/git/repositories/{repositoryId}";

    private Url GetBaseGitAddress(Guid projectId) => $" https://dev.azure.com/{organisation}/{projectId}/_apis/git";

    private Url GetBaseAddress() => $" https://dev.azure.com/{organisation}/_apis";

    private async ValueTask<IContinuablePage<T>> GetJsonPageAsync<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadAsAsync<VssJsonCollectionWrapper<List<T>>>(mediaFormatters);
        var continuationToken = response.Headers.TryGetValues("x-ms-continuationtoken", out var values)
            ? values.FirstOrDefault()
            : null;

        return new ContinuablePage<T>(continuationToken, items.Value);
    }

    private async ValueTask<IContinuablePage<TeamProjectReference>> GetProjectsAsync(string? continuationToken)
    {
        using var request = GetBaseAddress()
            .AppendPathSegment("projects")
            .SetQueryParam("continuationToken", continuationToken)
            .Get();

        using var response = await client.SendAsync(request);
        return await GetJsonPageAsync<TeamProjectReference>(response);
    }

    private async ValueTask<IEnumerable<GitPushRef>> GetPushesAsync(Guid projectId, Guid repoId, PageIndex page,
        DateRange dates)
    {
        using var request = GetBaseGitAddress(projectId, repoId)
            .AppendPathSegment("pushes")
            .SetPage(page)
            .SetDates(dates)
            .Get();

        using var response = await client.SendAsync(request);
        return await GetJsonPageAsync<GitPushRef>(response);
    }

    public IAsyncEnumerable<TeamProjectReference> GetProjectsAsync() =>
        new TokenPaginator<TeamProjectReference>(GetProjectsAsync).GetAsync();

    public async IAsyncEnumerable<GitRepositoryRef> GetRepositoriesAsync(Guid projectId)
    {
        using var request = GetBaseGitAddress(projectId)
            .AppendPathSegment("repositories")
            .Get();

        using var response = await client.SendAsync(request);
        var repos = await GetJsonPageAsync<GitRepositoryRef>(response);

        foreach (var repo in repos) yield return repo;
    }

    public IAsyncEnumerable<GitPushRef> GetPushesAsync(Guid projectId, Guid repoId, DateRange dates)
    {
        return new TokenlessPaginator<GitPushRef>(page => GetPushesAsync(projectId, repoId, page, dates))
            .GetAsync();
    }
}