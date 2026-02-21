using Desic.Data.Entities.Infrastructure;

namespace Desic.Data.Entities;

public class User : SoftDeletableEntity
{
    public required string Username { get; set; }
    public bool IsActive
    {
        get => _iaActive ?? true;
        set => _iaActive = value;
    }
    private bool? _iaActive;

    protected override Enums.EntityType EnumEntityType => Enums.EntityType.User;
}