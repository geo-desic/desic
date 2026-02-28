using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public record SystemTag(Guid Id, string Name) : IReadOnlyMinimalEntity
{
    public SystemEntityType SystemEntityType => SystemEntityTypes.Tag;

    public Tag ToEntity() => new()
    {
        Id = Id,
        CreatedByTypeId = Tag.ClassEntityType.Id,
        CreatedById = SystemTags.System.Id,
        ModifiedByTypeId = Tag.ClassEntityType.Id,
        ModifiedById = SystemTags.System.Id,
        Name = Name,
    };
}
