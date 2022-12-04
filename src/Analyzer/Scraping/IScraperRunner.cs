using System.Collections.Generic;
using System.Threading;

namespace Analyzer.Scraping;

/// <summary>
///     A factory class to create and run a scraper instance from a <see cref="IScraperDefinition" />.
/// </summary>
public interface IScraperRunner
{
    IAsyncEnumerable<IScraperDefinition> RunAsync(IScraperDefinition definition, CancellationToken ct);
}