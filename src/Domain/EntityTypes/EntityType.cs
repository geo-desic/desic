using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public class EntityType : MinimalEntity, IReadOnlyEntityType
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.EntityType;
}
