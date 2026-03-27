using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Labels;

public class Label : SoftDeletableEntity, IStaticEntityType, IReadOnlyNameable
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Label;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Name { get; set; }

    public const int MaxLengthName = 250;
}