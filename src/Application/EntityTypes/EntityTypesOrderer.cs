using Desic.Application.Common.Infrastructure;

namespace Desic.Application.EntityTypes;

internal sealed class EntityTypesOrderer : EnumQueryableOrderer<EntityTypesOrderingProperty, Domain.EntityTypes.EntityType>
{
    public EntityTypesOrderer()
    {
        Map(EntityTypesOrderingProperty.Key, x => x.Key);
        Map(EntityTypesOrderingProperty.Name, x => x.Name);
    }
}
