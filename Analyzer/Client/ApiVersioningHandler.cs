using Flurl;

namespace Analyzer.Client;

public class ApiVersioningHandler : DelegatingHandler
{
    private ApiVersioningHandler()
    {
    }

    public static ApiVersioningHandler Instance { get; } = new();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = Url.Parse(request.RequestUri?.ToString())
            .SetQueryParam("api-version", "6.0")
            .ToString();
        request.RequestUri = new Uri(url);

        return base.SendAsync(request, cancellationToken);
    }
}