using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public class Tag : SoftDeletableEntity, IStaticEntityType, IReadOnlyNameable
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Tag;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Name { get; set; }
    public string? Value { get; set; }

    public const int MaxLengthName = 250;
    public const int MaxLengthValue = 250;
}
