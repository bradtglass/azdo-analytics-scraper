using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Analyzer.Scraping;

public static class Util
{
    public static void AddIfUntracked<T>(this DbContext context, T entity)
        where T : class
    {
        var entry = context.Entry(entity);
        if (entry.State == EntityState.Detached)
            context.Set<T>().Add(entity);
    }

    public static async IAsyncEnumerable<IReadOnlyCollection<T>> ChunkAsync<T>(this IAsyncEnumerable<T> source,
        int chunkSize,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        List<T> list = new(chunkSize);

        await foreach (var item in source.WithCancellation(ct))
        {
            if (list.Count == chunkSize)
            {
                yield return list.ToImmutableArray();
                list.Clear();
            }

            list.Add(item);
        }

        if (list.Count > 0) yield return list.ToImmutableArray();
    }
}