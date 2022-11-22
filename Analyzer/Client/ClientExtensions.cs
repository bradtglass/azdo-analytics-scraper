using Flurl;

namespace Analyzer.Client;

public static class ClientExtensions
{
    public static HttpRequestMessage Get(this Url url) => new(HttpMethod.Get, url);

    public static Url SetPage(this Url url, PageIndex page)
        => url.SetQueryParam("$top", page.Top)
            .SetQueryParam("$skip", page.Skip);

    public static Url SetDates(this Url url, DateRange dates)
        => url.SetQueryParam("searchCriteria.toDate", dates.To.FormatDate())
            .SetQueryParam("searchCriteria.fromDate", dates.From.FormatDate());

    private static string FormatDate(this DateTimeOffset dto) =>
        dto.ToString("O");
}