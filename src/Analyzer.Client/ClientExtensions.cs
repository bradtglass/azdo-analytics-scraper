using System;
using System.Net.Http;
using Analyzer.Client.Paging;
using Flurl;

namespace Analyzer.Client;

public static class ClientExtensions
{
    public static HttpRequestMessage Get(this Url url) => new(HttpMethod.Get, url);
    
    public static HttpRequestMessage Post(this Url url) => new(HttpMethod.Post, url);

    public static Url SetPage(this Url url, PageIndex page, PageQueryFormat format)
    {
        var top = "top";
        var skip = "skip";

        switch (format)
        {
            case PageQueryFormat.DollarPrefix:
                top = '$' + top;
                skip = '$' + skip;
                break;
            case PageQueryFormat.SansDollar:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }

        return url.SetQueryParam(top, page.Top)
            .SetQueryParam(skip, page.Skip);
    }

    public static Url SetDates(this Url url, DateRange dates)
        => url.SetQueryParam("searchCriteria.toDate", dates.To.FormatDate())
            .SetQueryParam("searchCriteria.fromDate", dates.From.FormatDate());

    private static string FormatDate(this DateTimeOffset dto) =>
        dto.ToString("O");
}