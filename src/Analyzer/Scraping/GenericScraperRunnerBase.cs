using System.Collections.Generic;
using System.Threading;

namespace Analyzer.Scraping;

public abstract class GenericScraperRunnerBase
{
    public abstract IAsyncEnumerable<IScraperDefinition> RunAsync(CancellationToken ct);
}