using Analyzer.Client;
using Analyzer.Data;

namespace Analyzer.Scraping.Pushes;

public record PushScraperDefinition(DateRange Window, DevOpsGuid ProjectId, DevOpsGuid RepoId) : ScraperDefinitionBase(Window);