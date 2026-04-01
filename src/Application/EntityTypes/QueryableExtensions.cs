using Desic.Application.Common.Interfaces;

namespace Desic.Application.EntityTypes;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, EntityTypesFilter filter) where T : Domain.EntityTypes.IReadOnlyEntityType
    {
        var result = source;
        if (filter.Key != null)
        {
            result = result.Where(x => x.Key == filter.Key);
        }
        if (filter.Name != null)
        {
            result = result.Where(x => x.Name == filter.Name);
        }
        return result;
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IOrderingMethod<EntityTypesOrderingProperty> orderingMethod) where T : Domain.EntityTypes.IReadOnlyEntityType
    {
        return new EntityTypesOrderer<T>().ApplyOrderingMethod(source, orderingMethod);
    }

    public static IQueryable<EntityType> SelectToModel<T>(this IQueryable<T> source) where T : Domain.EntityTypes.IReadOnlyEntityType
        => source.Select(x => new EntityType { Name = x.Name, Key = x.Key });
}
