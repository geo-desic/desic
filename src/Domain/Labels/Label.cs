using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Labels;

public class Label : SoftDeletableEntity, IStaticEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Label;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Name { get; set; }
}