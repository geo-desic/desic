using Desic.Domain.EntityTypes;
using Desic.Domain.Shared.Entities;

namespace Desic.Domain.Tags;

public class ReadOnlyTag : IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    public IReadOnlyEntityType GetEntityType() => EntityTypes.SystemEntityTypes.Get(SystemEntityType.Tag);
}
