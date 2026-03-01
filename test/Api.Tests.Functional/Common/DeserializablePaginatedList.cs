namespace Desic.Api.Tests.Functional.Common;

public class DeserializablePaginatedList<T>
{
    public int StartIndex { get; set; }
    public int? TotalCount { get; set; }
    public List<T> Items { get; set; } = [];
}
