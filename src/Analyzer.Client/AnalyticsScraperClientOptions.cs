using System.Net.Http;
using Polly;

namespace Analyzer.Client;

public record AnalyticsScraperClientOptions(string Organisation, string AccessToken,
    AsyncPolicy<HttpResponseMessage> Policy);