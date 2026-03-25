namespace Desic.Application.Users;

public static class UserExtensions
{
    public static User ToDto(this Domain.Users.User source)
    {
        return new User(source)
        {
            Username = source.Username,
        };
    }
}
