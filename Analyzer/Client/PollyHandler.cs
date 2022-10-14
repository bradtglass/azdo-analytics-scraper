using Polly;

namespace Analyzer.Client;

public class PollyHandler : DelegatingHandler
{
    private readonly AsyncPolicy<HttpResponseMessage> policy;

    public PollyHandler(AsyncPolicy<HttpResponseMessage> policy)
    {
        this.policy = policy;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }
}