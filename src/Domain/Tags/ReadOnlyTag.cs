using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public class ReadOnlyTag : IStaticEntityType, IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public static IReadOnlyEntityType EntityType { get; } = SystemEntityTypes.Get(SystemEntityType.Tag);
    public IReadOnlyEntityType GetEntityType() => EntityType;

    public Guid Id { get; init; }
    public required string Name { get; init; }
}
