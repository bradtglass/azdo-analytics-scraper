﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Client.Paging;

public class TokenPaginator<T> : IPaginator<T>
{
    private readonly Func<string?, ValueTask<IContinuablePage<T>>> requestPage;

    public TokenPaginator(Func<string?, ValueTask<IContinuablePage<T>>> requestPage)
    {
        this.requestPage = requestPage;
    }

    public async IAsyncEnumerable<T> GetAsync()
    {
        string? token = null;
        do
        {
            var page = await requestPage(token);
            token = page.ContinuationToken;

            var hasItems = false;
            foreach (var item in page)
            {
                hasItems = true;
                yield return item;
            }

            if (!hasItems)
                token = null;
        } while (!string.IsNullOrEmpty(token));
    }
}