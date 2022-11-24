using System.Collections.Generic;

namespace Analyzer.Client.Paging;

public interface IContinuablePage<out T> : IEnumerable<T>
{
    string? ContinuationToken { get; }
}