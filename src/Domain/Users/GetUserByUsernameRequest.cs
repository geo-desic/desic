using MediatR;

namespace Desic.Domain.Users;

public class GetUserByUsernameRequest : IRequest<User?>
{
    public string? Username { get; set; }
}
