using Desic.Application.Common.Infrastructure;

namespace Desic.Application.EntityTypes;

internal sealed class EntityTypesOrderer<T> : EnumQueryableOrderer<EntityTypesOrderingProperty, T> where T : Domain.EntityTypes.IReadOnlyEntityType
{
    public EntityTypesOrderer()
    {
        Map(EntityTypesOrderingProperty.Key, x => x.Key);
        Map(EntityTypesOrderingProperty.Name, x => x.Name);
    }
}
