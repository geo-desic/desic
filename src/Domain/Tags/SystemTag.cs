using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public record SystemTag(Guid Id, string Name) : IReadOnlyMinimalEntity
{
    public SystemEntityType SystemEntityType => SystemEntityTypes.Tag;
}
