namespace Analyzer.Data;

public interface IHasDevOpsId<out T>
{
    T DevOpsId { get; }
}