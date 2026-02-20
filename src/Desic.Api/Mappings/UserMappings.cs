using Desic.Api.Dtos.Users;

namespace Desic.Api.Mappings;

public static class UserMappings
{
    public static User ToDto(this Business.Users.Models.User source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
        };
    }

    public static Business.Users.Models.UserCreate ToBusinessModel(this UserCreate source)
    {
        return new Business.Users.Models.UserCreate
        {
            Username = source.Username,
        };
    }
}
