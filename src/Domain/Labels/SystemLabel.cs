using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Labels;

public record SystemLabel(Guid Id, string Name) : IReadOnlyMinimalEntity
{
    public SystemEntityType SystemEntityType => SystemEntityTypes.Label;

    public Label ToEntity() => new()
    {
        Id = Id,
        CreatedByTypeId = Label.ClassEntityType.Id,
        CreatedById = SystemLabels.System.Id,
        ModifiedByTypeId = Label.ClassEntityType.Id,
        ModifiedById = SystemLabels.System.Id,
        Name = Name,
    };
}
