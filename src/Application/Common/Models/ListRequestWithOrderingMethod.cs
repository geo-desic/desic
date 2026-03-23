using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ListRequestWithOrderingMethod<T> : ListRequest, IOrderingMethod<T> where T: struct, Enum
{
    public T? OrderingMethod { get; set; }
}
