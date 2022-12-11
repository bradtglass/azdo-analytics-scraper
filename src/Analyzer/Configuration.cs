using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Analyzer;

public static class Configuration
{
    public static string GetConnectionString()
        => GetRequiredVariable("AZ_DO_DB", @"Server=(localdb)\DevOpsAnalytics;Initial Catalog=AZDOAnalytics;Integrated Security=true;");

    public static string GetAccessToken()
        => GetRequiredVariable("AZ_DO_PAT");

    public static string GetOrganisation()
        => GetRequiredVariable("AZ_DO_ORG");

    private static string GetRequiredVariable(string name, string? fallback = null)
    {
        var value = Environment.GetEnvironmentVariable(name);

        if (string.IsNullOrWhiteSpace(value))
            value = fallback;

        return value ?? throw new Exception($"Environment variable not set: {name}");
    }

    public static AsyncPolicy<HttpResponseMessage> GetPolicy()
    {
        return Policy<HttpResponseMessage>.HandleResult(r => r.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(3,
                (_, result, _) =>
                {
                    var pause = result.Result.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
                    return pause;
                }, (_, _, _, _) => Task.CompletedTask);
    }
}