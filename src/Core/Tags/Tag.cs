using Desic.Core.EntityTypes;
using Desic.Core.Shared.Entities;

namespace Desic.Core.Tags;

public class Tag : SoftDeletableEntity
{
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.Tag;
}
