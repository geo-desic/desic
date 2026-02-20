using Desic.EntityFrameworkCore.Entities;
using MediatR;

namespace Desic.EntityFrameworkCore.Users.Queries;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }
}
