using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Users;

public class User : SoftDeletableEntity, IStaticEntityType, IReadOnlyNameable, IReadOnlyUser
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.User;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public bool IsActive
    {
        get => _iaActive ?? true;
        set => _iaActive = value;
    }
    private bool? _iaActive;
    public required string Username { get; set; }

    string IReadOnlyNameable.Name => Username;

    public const int MaxLengthUsername = 100;
    public const int MinLengthUsername = 5;
    public const string RegexUsername = "^[a-zA-Z0-9]+([._-]?[a-zA-Z0-9]+)*$"; // alphanumeric characters, hyphens '-', and periods '.'; no consecutive special characters, nor at start/end
}