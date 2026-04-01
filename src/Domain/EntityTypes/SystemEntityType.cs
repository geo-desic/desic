using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.EntityTypes;

public record SystemEntityType(Guid Id, string Key, string Name) : IReadOnlyEntityType, IReadOnlyMinimalEntity, IReadOnlyNameable
{
    SystemEntityType IReadOnlyMinimalEntity.SystemEntityType => SystemEntityTypes.EntityType;

    public EntityType ToEntity() => new() { Id = Id, Key = Key, Name = Name };
}
