using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class OrderingMethod<T> : IOrderingMethod<T> where T : struct, Enum
{
    public List<OrderBy<T>> OrderBy { get; set; } = [];
    IReadOnlyList<IOrderBy<T>> IOrderingMethod<T>.OrderBy { get => OrderBy; }

    public static OrderingMethod<T> Default
    {
        get
        {
            return new OrderingMethod<T>
            {
                OrderBy = [new OrderBy<T> { Property = default, Ascending = true }],
            };
        }
    }
}
