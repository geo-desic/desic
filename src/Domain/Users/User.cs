using Desic.Domain.EntityTypes;
using Desic.Domain.Shared.Entities;

namespace Desic.Domain.Users;

public class User : SoftDeletableEntity
{
    public required string Username { get; set; }
    public bool IsActive
    {
        get => _iaActive ?? true;
        set => _iaActive = value;
    }
    private bool? _iaActive;

    protected override SystemEntityType EnumEntityType => SystemEntityType.User;
}