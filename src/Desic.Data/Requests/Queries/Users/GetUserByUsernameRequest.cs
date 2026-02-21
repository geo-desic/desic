using Desic.Data.Entities;
using MediatR;

namespace Desic.Data.Requests.Queries.Users;

public class GetUserByUsernameRequest : IRequest<User?>
{
    public string? Username { get; set; }
}
