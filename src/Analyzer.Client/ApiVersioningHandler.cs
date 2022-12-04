using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;

namespace Analyzer.Client;

public class ApiVersioningHandler : DelegatingHandler
{
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