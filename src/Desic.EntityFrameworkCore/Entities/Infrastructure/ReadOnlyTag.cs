using Desic.EntityFrameworkCore.Data;

namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

internal class ReadOnlyTag : IReadOnlyMinimalTag, IReadOnlyMinimalEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }

    public IReadOnlyEntityType GetEntityType() => EntityTypes.Get(Enums.EntityType.Tag);
}
