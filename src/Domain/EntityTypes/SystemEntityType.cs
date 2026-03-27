using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public record SystemEntityType(Guid Id, string Key, string Name) : IReadOnlyMinimalEntity, IReadOnlyNameable, IReadOnlyEntityTypeReferenceData
{
    SystemEntityType IReadOnlyMinimalEntity.SystemEntityType => SystemEntityTypes.EntityType;

    public EntityType ToEntity() => new() { Id = Id, Key = Key, Name = Name };

    bool IEquatable<IReadOnlyEntityTypeReferenceData>.Equals(IReadOnlyEntityTypeReferenceData? compare) => this.IsEquivalentTo(compare);
}
