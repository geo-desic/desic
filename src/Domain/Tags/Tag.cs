using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public class Tag : SoftDeletableEntity, IStaticEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Tag;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Name { get; set; }
}
