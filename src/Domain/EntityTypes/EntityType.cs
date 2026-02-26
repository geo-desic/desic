using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public class EntityType : BaseEntity, IStaticEntityType, IReadOnlyEntityType
{
    // this is needed because the IStaticEntityType.EntityType property has same name as this class
    private static readonly ReadOnlyEntityType _entityType = SystemEntityTypes.Get(SystemEntityType.EntityType);
    static IReadOnlyEntityType IStaticEntityType.EntityType { get; } = _entityType;
    public override IReadOnlyEntityType GetEntityType() => _entityType;

    public required string Name { get; set; }
}
