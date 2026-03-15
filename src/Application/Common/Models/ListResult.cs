using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ListResult<T> : IListResult<T>
{
    public ListResult() { }
    public ListResult(IReadOnlyCollection<T> items)
    {
        Items = items;
    }
    public ListResult(IListResult<T> listResult) : base()
    {
        Items = listResult.Items;
        StartIndex = listResult.StartIndex;
        TotalCount = listResult.TotalCount;
    }
    public int StartIndex { get; init; }
    public int? TotalCount { get; init; }
    public IReadOnlyCollection<T> Items { get; init; } = [];
}
