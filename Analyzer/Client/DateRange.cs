namespace Analyzer.Client;

public readonly struct DateRange
{
    public DateRange(DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new ArgumentOutOfRangeException(nameof(to), "To must be less than from");

        From = from;
        To = to;
    }

    public DateOnly From { get; }

    public DateOnly To { get; }
}