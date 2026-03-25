using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class FilterableOrderableListRequest<F, O> : ListRequest, IOrderingMethod<O>, IFilterable<F>
    where O : struct, Enum
    where F : class, new()
{
    public F Filter { get; set; } = new();

    public virtual O OrderingMethod { get; set; } = default;
}
