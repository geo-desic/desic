namespace Desic.Domain.Users;

public interface IReadOnlyUser
{
    bool IsActive { get; }
    string Username { get; }
}
