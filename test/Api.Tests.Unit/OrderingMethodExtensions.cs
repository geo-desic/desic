using Desic.Application.Common.Interfaces;

namespace Desic.Api.Tests.Unit;

public static class OrderingMethodExtensions
{
    public static bool IsEquivalentTo<T>(this IOrderingMethod<T> source, IOrderingMethod<T> compare) where T : struct, Enum
    {
        return source.OrderBy.Count == compare.OrderBy.Count && !source.OrderBy.Where((s, i) => s.Ascending != compare.OrderBy[i].Ascending || !s.Property.Equals(compare.OrderBy[i].Property)).Any();
    }
}
