namespace Analyzer;

public class TokenlessPaginator<T> : IPaginator<T>
{
    public delegate ValueTask<IEnumerable<T>> GetPageAsyncDelegate(int top, int skip);

    private readonly GetPageAsyncDelegate getPageDelegate;

    public TokenlessPaginator(GetPageAsyncDelegate getPageDelegate)
    {
        this.getPageDelegate = getPageDelegate;
    }

    public async IAsyncEnumerable<T> GetAsync()
    {
        const int pageSize = 100;
        var page = 0;

        int iteratedCount;
        do
        {
            iteratedCount = 0;

            var items = await getPageDelegate(pageSize, page * pageSize).ConfigureAwait(false);
            page++;

            foreach (var item in items)
            {
                iteratedCount++;
                yield return item;
            }
        } while (iteratedCount == pageSize);
    }
}