using System.Collections.Generic;

namespace Analyzer.Client.Paging;

public interface IPaginator<T>
{
    IAsyncEnumerable<T> GetAsync();
}