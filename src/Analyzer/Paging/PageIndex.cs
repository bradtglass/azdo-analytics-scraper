namespace Analyzer.Paging;

public readonly struct PageIndex
{
    public static readonly PageIndex DefaultFirstPage = new(0, 1);

    public override string ToString() => $"[{Index}] {Index * Size}(->{Index * Size + Size - 1})";

    public PageIndex(int index, int size)
    {
        Index = index;
        Size = size;
    }

    public int Index { get; }

    public int Size { get; }

    public int Skip => Size * Index;

    public int Top => Size;

    public static PageIndex operator ++(PageIndex current)
        => new(current.Index + 1, current.Size);
}