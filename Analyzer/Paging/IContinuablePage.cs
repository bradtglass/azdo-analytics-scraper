namespace Analyzer;

public interface IContinuablePage<out T>
{
    string? ContinuationToken { get; }

    IEnumerable<T> Items { get; }
}