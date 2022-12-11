using Analyzer.Client;

namespace Analyzer.Scraping.Projects;

public record ProjectScraperDefinition(DateRange Window) : ScraperDefinitionBase(Window);