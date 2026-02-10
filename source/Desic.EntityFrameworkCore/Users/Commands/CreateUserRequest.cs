using Desic.EntityFrameworkCore.Entities;
using MediatR;

namespace Desic.EntityFrameworkCore.Users.Commands;

public class CreateUserRequest : IRequest<Guid>
{
    public required User User { get; set; }
}
