using Analyzer.Client;

namespace Analyzer.Scraping;

public record ScraperDefinitionBase(DateRange Window) : IScraperDefinition;