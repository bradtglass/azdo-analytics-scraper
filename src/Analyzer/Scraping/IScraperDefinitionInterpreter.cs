using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer.Scraping;

public interface IScraperDefinitionInterpreter
{
    ValueTask<IScraperDefinition> InterpretAsync(Stream stream, CancellationToken ct);
}