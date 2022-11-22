namespace Analyzer;

public class TokenlessPaginator<T> : IPaginator<T>
{
    public delegate ValueTask<IEnumerable<T>> GetPageAsyncDelegate(PageIndex page);

    private readonly GetPageAsyncDelegate getPageDelegate;

    public TokenlessPaginator(GetPageAsyncDelegate getPageDelegate)
    {
        this.getPageDelegate = getPageDelegate;
    }

    public async IAsyncEnumerable<T> GetAsync()
    {
        var page = PageIndex.DefaultFirstPage;

        bool hasItems;
        do
        {
            var items = await getPageDelegate(page++).ConfigureAwait(false);

            hasItems = false;
            foreach (var item in items)
            {
                hasItems = true;
                yield return item;
            }
        } while (hasItems);
    }
}