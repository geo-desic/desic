using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.EntityTypes;

public record SystemEntityType(Guid Id, string Key, string Name) : IReadOnlyMinimalEntity, IReadOnlyNameable, IReadOnlyEntityType
{
    SystemEntityType IReadOnlyMinimalEntity.SystemEntityType => SystemEntityTypes.EntityType;

    public EntityType ToEntity() => new() { Id = Id, Key = Key, Name = Name };

    bool IEquatable<IReadOnlyEntityType>.Equals(IReadOnlyEntityType? compare) => this.IsEquivalentTo(compare);
}
