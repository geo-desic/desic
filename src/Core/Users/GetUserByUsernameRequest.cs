using MediatR;

namespace Desic.Core.Users;

public class GetUserByUsernameRequest : IRequest<User?>
{
    public string? Username { get; set; }
}
