using Desic.Data.EntityTypes;
using Desic.Data.Shared.Entities;

namespace Desic.Data.Tags;

public class Tag : SoftDeletableEntity
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.Tag;
}
