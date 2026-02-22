using Desic.Data.EntityTypes;
using Desic.Data.Shared.Entities;

namespace Desic.Data.Tags;

public class ReadOnlyTag : IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    public IReadOnlyEntityType GetEntityType() => EntityTypes.SystemEntityTypes.Get(SystemEntityType.Tag);
}
