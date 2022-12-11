using Analyzer.Client;
using Analyzer.Data;

namespace Analyzer.Scraping.Repos;

public record RepoScraperDefinition(DateRange Window, DevOpsGuid ProjectId) : ScraperDefinitionBase(Window);