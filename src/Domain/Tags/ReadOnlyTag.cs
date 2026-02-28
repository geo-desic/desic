using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public class ReadOnlyTag : IStaticEntityType, IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Tag;
    public SystemEntityType SystemEntityType => ClassEntityType;

    public Guid Id { get; init; }
    public required string Name { get; init; }
}
