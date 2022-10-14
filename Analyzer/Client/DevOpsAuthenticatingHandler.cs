using System.Net.Http.Headers;
using System.Text;

namespace Analyzer.Client;

public class DevOpsAuthenticatingHandler : DelegatingHandler
{
    private readonly AuthenticationHeaderValue header;

    public DevOpsAuthenticatingHandler(string pat)
    {
        header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = header;
        return base.SendAsync(request, cancellationToken);
    }
}