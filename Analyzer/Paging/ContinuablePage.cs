namespace Analyzer;

public record ContinuablePage<T>(string? ContinuationToken, IEnumerable<T> Items) : IContinuablePage<T>;