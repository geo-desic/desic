using Desic.EntityFrameworkCore.Entities.Infrastructure;

namespace Desic.EntityFrameworkCore.Entities;

public class Tag : ModifiableEntity
{
    public required string Name { get; set; }

    protected override Enums.EntityType EnumEntityType => Enums.EntityType.Tag;
}
