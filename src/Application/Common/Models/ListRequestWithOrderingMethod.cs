using Desic.Application.Common.Interfaces;
using System.ComponentModel;

namespace Desic.Application.Common.Models;

public class ListRequestWithOrderingMethod<T> : ListRequest, IOrderingMethod<T> where T: struct, Enum
{
    [Description("The requested ordering method to be applied to the queried and returned items")]
    public virtual T OrderingMethod { get; set; } = default;
}
