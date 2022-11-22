using System.Collections;

namespace Analyzer;

public record ContinuablePage<T>(string? ContinuationToken, IEnumerable<T> Items) : IContinuablePage<T>
{
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}