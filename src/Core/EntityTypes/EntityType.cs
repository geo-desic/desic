using Desic.Core.Shared.Entities;

namespace Desic.Core.EntityTypes;

public class EntityType : MinimalEntity, IReadOnlyEntityType, IReadOnlyMinimalEntity
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.EntityType;
    public override IReadOnlyEntityType GetEntityType() => this;
}
