using Desic.Application.Common.Helpers;

namespace Desic.Application.Users;

public static class UserHelpers
{
    public static User ToDto(this Domain.Users.User user)
    {
        var result = new User
        {
            Id = user.Id,
            Username = user.Username,
        };
        result.MapCreatedModifiedDeleted(user);
        return result;
    }
}
