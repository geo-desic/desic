namespace Desic.Application.Common.Interfaces;

public interface IListResult<T>
{
    int StartIndex { get; init; }
    int? TotalCount { get; init; }
    IReadOnlyCollection<T> Items { get; init; }
}
