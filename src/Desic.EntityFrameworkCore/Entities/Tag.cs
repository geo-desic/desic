using Desic.EntityFrameworkCore.Entities.Infrastructure;

namespace Desic.EntityFrameworkCore.Entities;

public class Tag : SoftDeletableEntity
{
    public required string Name { get; set; }

    protected override Enums.EntityType EnumEntityType => Enums.EntityType.Tag;
}
