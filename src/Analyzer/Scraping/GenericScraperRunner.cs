using System.Collections.Generic;
using System.Threading;

namespace Analyzer.Scraping;

/// <summary>
///     This and <see cref="GenericScraperRunnerBase" /> purely help to bridge the divide between the generic and
///     non-generic code that is used in <see cref="IScraperRunner" />.
/// </summary>
public class GenericScraperRunner<T> : GenericScraperRunnerBase
    where T : class, IScraperDefinition
{
    private readonly T definition;
    private readonly IScraper<T> scraper;

    public GenericScraperRunner(T definition, IScraper<T> scraper)
    {
        this.definition = definition;
        this.scraper = scraper;
    }

    public override IAsyncEnumerable<IScraperDefinition> RunAsync(CancellationToken ct) =>
        scraper.ScrapeAsync(definition, ct);
}