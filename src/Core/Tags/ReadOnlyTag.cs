using Desic.Core.EntityTypes;
using Desic.Core.Shared.Entities;

namespace Desic.Core.Tags;

public class ReadOnlyTag : IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    public IReadOnlyEntityType GetEntityType() => EntityTypes.SystemEntityTypes.Get(SystemEntityType.Tag);
}
