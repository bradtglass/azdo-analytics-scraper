using System.Collections.Generic;
using System.Threading;

namespace Analyzer.Scraping;

/// <summary>
///     An object that will scrape data from a single entity like a repository.
/// </summary>
public interface IScraper<TDefinition>
    where TDefinition : class
{
    /// <summary>
    ///     Scrapes the data and asynchronously returns any child scraper definitions which can be run immediately.
    /// </summary>
    IAsyncEnumerable<IScraperDefinition> ScrapeAsync(TDefinition definition, CancellationToken ct);
}