using Desic.Application.Common.Interfaces;
using System.ComponentModel;

namespace Desic.Application.Common.Models;

public class FilterableOrderableListRequest<F, O> : ListRequest, IOrderingMethod<O>, IFilterable<F>
    where O: struct, Enum
    where F : class, new()
{
    [Description("The requested filters for the queried and returned items which iare applied before any ordering or pagination")]
    public F Filters { get; set; } = new();

    [Description("The requested ordering method for the queried and returned items which is applied after any filtering but before any pagination")]
    public virtual O OrderingMethod { get; set; } = default;
}
