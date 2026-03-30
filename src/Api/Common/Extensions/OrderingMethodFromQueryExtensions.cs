using Desic.Api.Dtos;
using Desic.Application.Common.Models;

namespace Desic.Api.Common.Extensions;

public static class OrderingMethodFromQueryExtensions
{
    public static bool TryConvertToOrderingMethod<T>(this OrderingMethodFromQuery<T> source, out OrderingMethod<T> result) where T : struct, Enum
    {
        if (source.OrderBy.Count == 0 && source.Asc.Count == 0)
        {
            result = OrderingMethod<T>.Default;
            return true;
        }
        // source.OrderBy.Count controls the number of items in the result regardless of source.Asc.Count
        var count = source.OrderBy.Count;
        result = new();
        List<bool> itemsAsc;
        if (source.Asc.Count > count) return false; // non-sensical case, do not attempt conversion
        else
        {
            itemsAsc = [.. source.Asc];
            if (source.Asc.Count < count)
            {
                // all missing items will be added as true (ascending)
                for (var i = 0; i < count - source.Asc.Count; ++i)
                {
                    itemsAsc.Add(true);
                }
            }
        }
        for (var i = 0; i < count; ++i)
        {
            result.OrderBy.Add(new() { Ascending = itemsAsc[i], Property = source.OrderBy[i] });
        }
        return true;
    }
}
