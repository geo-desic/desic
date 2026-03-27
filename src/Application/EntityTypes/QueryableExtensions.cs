using Desic.Application.EntityTypes;

namespace Desic.Application.EntityTypes;

public static class QueryableExtensions
{
    public static IQueryable<Domain.EntityTypes.EntityType> ApplyFilter(this IQueryable<Domain.EntityTypes.EntityType> source, EntityTypesFilter filter)
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

    public static IOrderedQueryable<Domain.EntityTypes.EntityType> OrderBy(this IQueryable<Domain.EntityTypes.EntityType> source, EntityTypesOrderingMethod? orderingMethod)
    {
        return (orderingMethod ?? default) switch
        {
            EntityTypesOrderingMethod.KeyAsc => source.OrderBy(x => x.Key),
            EntityTypesOrderingMethod.KeyDesc => source.OrderByDescending(x => x.Key),
            EntityTypesOrderingMethod.NameAsc => source.OrderBy(x => x.Name),
            EntityTypesOrderingMethod.NameDesc => source.OrderByDescending(x => x.Name),
            _ => source.OrderBy(x => x.Name),
        };
    }

    public static IQueryable<EntityType> SelectToModel(this IQueryable<Domain.EntityTypes.EntityType> source) => source.Select(x => new EntityType { Name = x.Name, Key = x.Key });
}
