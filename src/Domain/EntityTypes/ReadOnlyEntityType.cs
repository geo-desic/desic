using Desic.Domain.Common.Entities;

namespace Desic.Domain.EntityTypes;

public class ReadOnlyEntityType : IStaticEntityType, IReadOnlyEntityType, IReadOnlyMinimalEntity
{
    public static IReadOnlyEntityType EntityType { get; } = SystemEntityTypes.Get(SystemEntityType.EntityType);
    public IReadOnlyEntityType GetEntityType() => EntityType;

    public Guid Id { get; init; }
    public required string Name { get; init; }
}
