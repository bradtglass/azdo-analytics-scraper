namespace Analyzer;

public interface IPaginator<T>
{
    IAsyncEnumerable<T> GetAsync();
}