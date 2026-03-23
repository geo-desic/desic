namespace Desic.Application.EntityTypes;

public static class QueryableExtensions
{
    public static IOrderedQueryable<EntityType> OrderBy(this IQueryable<EntityType> query, EntityTypesOrderingMethod? orderingMethod)
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
}
