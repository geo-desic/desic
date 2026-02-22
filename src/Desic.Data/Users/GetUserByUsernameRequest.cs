using MediatR;

namespace Desic.Data.Users;

public class GetUserByUsernameRequest : IRequest<User?>
{
    public string? Username { get; set; }
}
