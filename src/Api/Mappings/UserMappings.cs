using Desic.Api.Dtos.Users;

namespace Desic.Api.Mappings;

public static class UserMappings
{
    public static User ToDto(this Application.Users.User source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
        };
    }

    public static Application.Users.Create.UserCreate ToBusinessModel(this UserCreate source)
    {
        return new Application.Users.Create.UserCreate
        {
            Username = source.Username,
        };
    }
}
