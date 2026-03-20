namespace Desic.Api.Tests.Functional.Common.Models;

public class DeserializableListResult<T>
{
    public int StartIndex { get; set; }
    public int? TotalCount { get; set; }
    public List<T> Items { get; set; } = [];
}
