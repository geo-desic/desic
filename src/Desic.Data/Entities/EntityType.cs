using Desic.Data.Entities.Infrastructure;

namespace Desic.Data.Entities;

public class EntityType : MinimalEntity, IReadOnlyEntityType, IReadOnlyMinimalEntity
{
    public required string Name { get; set; }

    protected override Enums.EntityType EnumEntityType => Enums.EntityType.EntityType;
    public override IReadOnlyEntityType GetEntityType() => this;
}
