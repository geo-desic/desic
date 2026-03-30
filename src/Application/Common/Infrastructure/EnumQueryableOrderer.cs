using Desic.Application.Common.Interfaces;
using System.Linq.Expressions;

namespace Desic.Application.Common.Infrastructure;

internal abstract class EnumQueryableOrderer<TEnum, TSource> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, IQueryableOrderer<TSource>> _mapping = [];

    protected void Map<TKey>(TEnum property, Expression<Func<TSource, TKey>> keySelector)
    {
        _mapping.Add(property, new QueryableOrderer<TSource, TKey> { KeySelector = keySelector });
    }

    public IOrderedQueryable<TSource> ApplyOrderingMethod(IQueryable<TSource> query, IOrderingMethod<TEnum> orderingMethod)
    {
        if (orderingMethod.OrderBy.Count == 0)
        {
            throw new ArgumentException($"{nameof(orderingMethod.OrderBy)} must contain at least one element", nameof(orderingMethod));
        }
        if (orderingMethod.OrderBy.Count > OrderingMethods.MaximumOrderByCount)
        {
            throw new ArgumentException($"{nameof(orderingMethod.OrderBy)} must contain at most {OrderingMethods.MaximumOrderByCount} elements", nameof(orderingMethod));
        }
        IOrderedQueryable<TSource> result = null!;
        bool first = true;
        foreach (var orderBy in orderingMethod.OrderBy)
        {
            if (!_mapping.TryGetValue(orderBy.Property, out var orderer))
            {
                throw new NotSupportedException($"Property has not been mapped: '{orderBy.Property}'");
            }
            if (first)
            {
                first = false;
                result = orderer.OrderBy(query: query, ascending: orderBy.Ascending);
            }
            else
            {
                result = orderer.ThenBy(query: result, ascending: orderBy.Ascending);
            }
        }
        return result;
    }
}
