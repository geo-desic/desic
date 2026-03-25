using Desic.Application.EntityTypes;

namespace Desic.Application.EntityTypes;

public static class QueryableExtensions
{
    public static IOrderedQueryable<Domain.EntityTypes.EntityType> OrderBy(this IQueryable<Domain.EntityTypes.EntityType> query, EntityTypesOrderingMethod? orderingMethod)
    {
        return (orderingMethod ?? EntityTypesOrderingMethod.NameAsc) switch
        {
            EntityTypesOrderingMethod.KeyAsc => query.OrderBy(x => x.Key),
            EntityTypesOrderingMethod.KeyDesc => query.OrderByDescending(x => x.Key),
            EntityTypesOrderingMethod.NameAsc => query.OrderBy(x => x.Name),
            EntityTypesOrderingMethod.NameDesc => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name),
        };
    }

    public static IQueryable<Domain.EntityTypes.EntityType> ApplyFilter(this IQueryable<Domain.EntityTypes.EntityType> query, EntityTypesFilter filter)
    {
        var result = query;
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
}
