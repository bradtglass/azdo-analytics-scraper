namespace Analyzer;

public interface IContinuablePage<out T> : IEnumerable<T>
{
    string? ContinuationToken { get; }
}