using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public class Tag : SoftDeletableEntity, IStaticEntityType
{
    public static IReadOnlyEntityType EntityType { get; } = SystemEntityTypes.Get(SystemEntityType.Tag);
    public override IReadOnlyEntityType GetEntityType() => EntityType;

    public required string Name { get; set; }
}
