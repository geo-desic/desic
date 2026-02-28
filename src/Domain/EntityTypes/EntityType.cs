using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public class EntityType : BaseEntity, IStaticEntityType, IReadOnlyEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.EntityType;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Key { get; set; }
    public required string Name { get; set; }
}
