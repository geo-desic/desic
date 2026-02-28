using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Users;

public class User : SoftDeletableEntity, IStaticEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.User;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Username { get; set; }
    public bool IsActive
    {
        get => _iaActive ?? true;
        set => _iaActive = value;
    }
    private bool? _iaActive;
}