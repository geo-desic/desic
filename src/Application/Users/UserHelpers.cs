namespace Desic.Application.Users;

public static class UserHelpers
{
    public static User ToDto(this Domain.Users.User user)
    {
        return new User(user)
        {
            Username = user.Username,
        };
    }
}
