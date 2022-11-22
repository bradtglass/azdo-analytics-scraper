using System.Collections.Generic;

namespace Analyzer.Paging;

public interface IContinuablePage<out T> : IEnumerable<T>
{
    string? ContinuationToken { get; }
}