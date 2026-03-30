using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class FilterableOrderableListRequest<F, O> : ListRequest, IFilterable<F>
    where O : struct, Enum
    where F : class, new()
{
    public F Filter { get; set; } = new();

    public IOrderingMethod<O> OrderingMethod { get; set; } = OrderingMethod<O>.Default;
}
