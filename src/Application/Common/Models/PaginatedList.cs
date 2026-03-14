namespace Desic.Application.Common.Models;

public class PaginatedList<T>(IReadOnlyCollection<T> items, int startIndex, int? totalCount = null)
{
    public int StartIndex { get; } = startIndex;
    public int? TotalCount { get; } = totalCount;
    public IReadOnlyCollection<T> Items { get; } = items;
}
