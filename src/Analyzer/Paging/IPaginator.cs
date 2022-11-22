using System.Collections.Generic;

namespace Analyzer.Paging;

public interface IPaginator<T>
{
    IAsyncEnumerable<T> GetAsync();
}