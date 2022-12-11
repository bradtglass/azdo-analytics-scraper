using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Analyzer.Client.Paging;
using Flurl;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;

namespace Analyzer.Client;

public sealed class AnalyticsScraperClient : IDisposable
{
    private readonly HttpClient client;
    private readonly MediaTypeFormatter[] mediaFormatters = { new VssJsonMediaTypeFormatter() };

    public AnalyticsScraperClient(AnalyticsScraperClientOptions options)
    {
        Organisation = options.Organisation;
        client = HttpClientFactory.Create(new ApiVersioningHandler(),
            new DevOpsAuthenticatingHandler(options.AccessToken),
            new PollyHandler(options.Policy));
    }

    public string Organisation { get; }

    public void Dispose()
    {
        client.Dispose();
    }

    private Url GetBaseGitAddress(Guid projectId, Guid repositoryId) =>
        $"https://dev.azure.com/{Organisation}/{projectId}/_apis/git/repositories/{repositoryId}";

    private Url GetBaseGitAddress(Guid projectId) => $"https://dev.azure.com/{Organisation}/{projectId}/_apis/git";

    private Url GetBaseAddress() => $"https://dev.azure.com/{Organisation}/_apis";

    private Url GetVssBaseAddress() => $"https://vssps.dev.azure.com/{Organisation}/_apis";

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
            .SetPage(page, PageQueryFormat.DollarPrefix)
            .SetDates(dates)
            .Get();

        using var response = await client.SendAsync(request);
        return await ReadJsonPageAsync<GitPush>(response);
    }

    private async ValueTask<IEnumerable<GitPullRequest>> GetPullRequestsAsync(Guid projectId, Guid repoId, PageIndex page)
    {
        using var request = GetBaseGitAddress(projectId, repoId)
            .AppendPathSegment("pullrequests")
            .SetPage(page, PageQueryFormat.DollarPrefix)
            .Get();

        using var response = await client.SendAsync(request);
        return await ReadJsonPageAsync<GitPullRequest>(response);
    }

    private async ValueTask<IEnumerable<GitCommit>> GetPushCommitsAsync(Guid projectId, Guid repoId, int pushId,
        PageIndex page)
    {
        using var request = GetBaseGitAddress(projectId, repoId)
            .AppendPathSegment("commits")
            .SetPage(page, PageQueryFormat.SansDollar)
            .SetQueryParam("pushId", pushId)
            .Get();

        using var response = await client.SendAsync(request);
        return await ReadJsonPageAsync<GitCommit>(response);
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

    public IAsyncEnumerable<GitCommit> GetPushCommitsAsync(Guid projectId, Guid repoId, int pushId)
    {
        return new TokenlessPaginator<GitCommit>(page => GetPushCommitsAsync(projectId, repoId, pushId, page))
            .GetAsync();
    }

    public async ValueTask<Identity?> FindIdentityByEmailAsync(string emailAddress)
    {
        using var request = GetVssBaseAddress()
            .AppendPathSegment("identities")
            .SetQueryParam("searchFilter", "MailAddress")
            .SetQueryParam("filterValue", emailAddress)
            .Get();

        using var response = await client.SendAsync(request);
        var ids = await ReadJsonPageAsync<Identity>(response);

        return ids.FirstOrDefault();
    }


    public async ValueTask<List<GitPullRequest>> GetPullRequestsFromMergeCommitsAsync(Guid projectId, Guid repoId, IEnumerable<string> mergeCommitShas)
    {
        using var request = GetBaseGitAddress(projectId, repoId)
            .AppendPathSegment("pullrequestquery")
            .Get();

        var query = new PullRequestQuery(new List<PullRequestQueryInput>()
        {
            new(mergeCommitShas.ToList(), GitPullRequestQueryType.LastMergeCommit)
        }, new List<GitPullRequest>());
        var bytes = JsonSerializer.SerializeToUtf8Bytes(query);
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json", "utf-8");
        request.Content = content;
        
        using var response = await client.SendAsync(request);
        var result = await ReadJsonAsync<GitPullRequestQuery>(response);
        return result.Results.SelectMany(d => d.Values).SelectMany(pr => pr).ToList();
    }

    private record PullRequestQuery([property:JsonPropertyName("queries")]List<PullRequestQueryInput> Queries,
        [property:JsonPropertyName("results")]List<GitPullRequest> Results);
    
    private record PullRequestQueryInput([property:JsonPropertyName("items")]List<string> Shas,
        [property:JsonPropertyName("type")] [property:JsonConverter(typeof(JsonStringEnumConverter))]GitPullRequestQueryType Type);
}