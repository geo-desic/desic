using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public class EntityType : BaseEntity, IStaticEntityType, IReadOnlyNameable, IReadOnlyEntityTypeReferenceData
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.EntityType;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Key { get; set; }
    public required string Name { get; set; }

    public const int LengthKey = 4;
    public const int MaxLengthName = 50;

    bool IEquatable<IReadOnlyEntityTypeReferenceData>.Equals(IReadOnlyEntityTypeReferenceData? compare) => this.IsEquivalentTo(compare);
}
