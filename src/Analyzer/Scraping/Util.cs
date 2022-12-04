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
}