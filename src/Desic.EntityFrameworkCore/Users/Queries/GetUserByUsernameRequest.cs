using Desic.EntityFrameworkCore.Entities;
using MediatR;

namespace Desic.EntityFrameworkCore.Users.Queries;

public class GetUserByUsernameRequest : IRequest<User?>
{
    public string? Username { get; set; }
}
