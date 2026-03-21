using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ListResult<T> : IListResult<T>
{
    public int StartIndex { get; init; }
    public int? TotalCount { get; init; }
    public IReadOnlyCollection<T> Items { get; init; } = [];
}
