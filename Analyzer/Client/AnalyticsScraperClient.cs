using System.Net.Http.Formatting;
using Flurl;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Polly;

namespace Analyzer.Client;

public sealed class AnalyticsScraperClient : IDisposable
{
    private readonly string baseAddress;
    private readonly HttpClient client;
    private readonly MediaTypeFormatter[] mediaFormatters = { new VssJsonMediaTypeFormatter() };

    public AnalyticsScraperClient(string organisation, string pat, AsyncPolicy<HttpResponseMessage> policy)
    {
        client = HttpClientFactory.Create(ApiVersioningHandler.Instance,
            new DevOpsAuthenticatingHandler(pat),
            new PollyHandler(policy));

        // Use this instead of client.BaseAddress because we're using Flurl
        baseAddress = $" https://dev.azure.com/{organisation}/_apis";
    }

    private async ValueTask<IContinuablePage<T>> GetJsonPageAsync<T>(HttpResponseMessage response)
    {
        var items = await response.Content.ReadAsAsync<VssJsonCollectionWrapper<List<T>>>(mediaFormatters);
        var continuationToken = response.Headers.TryGetValues("x-ms-continuationtoken", out var values)
            ? values.FirstOrDefault()
            : null;

        return new ContinuablePage<T>(continuationToken, items.Value);
    }

    private async ValueTask<IContinuablePage<TeamProjectReference>> GetProjectsAsync(string? continuationToken)
    {
        using var request = GenerateGetRequest(u => u.AppendPathSegment("projects")
            .SetQueryParam("continuationToken", continuationToken));

        using var response = await client.SendAsync(request);
        return await GetJsonPageAsync<TeamProjectReference>(response);
    }

    public IAsyncEnumerable<TeamProjectReference> GetProjectsAsync()
    {
        return new TokenPaginator<TeamProjectReference>(GetProjectsAsync).GetAsync();
    }

    private HttpRequestMessage GenerateGetRequest(Action<Url> configureUrl)
    {
        var url = new Url(baseAddress);
        configureUrl(url);

        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    public void Dispose()
    {
        client.Dispose();
    }
}