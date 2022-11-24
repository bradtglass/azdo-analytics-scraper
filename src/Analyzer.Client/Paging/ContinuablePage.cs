using System.Collections;
using System.Collections.Generic;

namespace Analyzer.Client.Paging;

public record ContinuablePage<T>(string? ContinuationToken, IEnumerable<T> Items) : IContinuablePage<T>
{
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}