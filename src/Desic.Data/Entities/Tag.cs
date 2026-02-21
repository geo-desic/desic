using Desic.Data.Entities.Infrastructure;

namespace Desic.Data.Entities;

public class Tag : SoftDeletableEntity
{
    public required string Name { get; set; }

    protected override Enums.EntityType EnumEntityType => Enums.EntityType.Tag;
}
