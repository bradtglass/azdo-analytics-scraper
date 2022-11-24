using System;

namespace Analyzer.Client;

public readonly struct DateRange
{
    public DateRange(DateTimeOffset from, DateTimeOffset to)
    {
        if (from > to)
            throw new ArgumentOutOfRangeException(nameof(to), "To must be less than from");

        From = from;
        To = to;
    }

    public DateTimeOffset From { get; }

    public DateTimeOffset To { get; }
}