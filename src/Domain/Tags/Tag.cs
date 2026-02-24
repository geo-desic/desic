using Desic.Domain.EntityTypes;
using Desic.Domain.Shared.Entities;

namespace Desic.Domain.Tags;

public class Tag : SoftDeletableEntity
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.Tag;
}
