using Analyzer.Data;

namespace Analyzer.Scraping.PullRequests;

public record PullRequestScraperDefinition (DevOpsGuid ProjectId, DevOpsGuid RepoId) : IScraperDefinition;