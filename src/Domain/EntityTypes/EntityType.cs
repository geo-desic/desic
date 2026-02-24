using Desic.Domain.Shared.Entities;

namespace Desic.Domain.EntityTypes;

public class EntityType : MinimalEntity, IReadOnlyEntityType, IReadOnlyMinimalEntity
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.EntityType;
    public override IReadOnlyEntityType GetEntityType() => this;
}
