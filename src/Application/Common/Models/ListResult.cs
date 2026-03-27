using Desic.Application.Common.Interfaces;
using System.ComponentModel;

namespace Desic.Application.Common.Models;

[Description("The result of a list request")]
public class ListResult<T> : IListResult<T>
{
    [Description("The start index (aka offset) of the potentially filtered items that is applicable to pagination")]
    public int StartIndex { get; init; }
    [Description("If total count inclusion is requested and allowed, the total count of matching items that in many cases will be larger than the count of paginated items returned")]
    public int? TotalCount { get; init; }
    [Description("The paginated items")]
    public IReadOnlyCollection<T> Items { get; init; } = [];
}
