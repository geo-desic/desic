using Desic.Api.Dtos.Users;

namespace Desic.Api.Mappings;

public static class UserMappings
{
    public static User ToDto(this Business.Models.Users.User source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
        };
    }

    public static Business.Models.Users.UserCreate ToBusinessModel(this UserCreate source)
    {
        return new Business.Models.Users.UserCreate
        {
            Username = source.Username,
        };
    }
}
