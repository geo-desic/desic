using Desic.Application.Common.Interfaces;

namespace Desic.Application.EntityTypes;

public static class QueryableExtensions
{
    private static readonly Lazy<EntityTypesOrderer> _orderer = new();

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

    public static IOrderedQueryable<Domain.EntityTypes.EntityType> OrderBy(this IQueryable<Domain.EntityTypes.EntityType> source, IOrderingMethod<EntityTypesOrderingProperty> orderingMethod)
    {
        return _orderer.Value.ApplyOrderingMethod(source, orderingMethod);
    }

    public static IQueryable<EntityType> SelectToModel(this IQueryable<Domain.EntityTypes.EntityType> source) => source.Select(x => new EntityType { Name = x.Name, Key = x.Key });
}
