using Desic.Data.Entities;
using MediatR;

namespace Desic.Data.Requests.Commands.Users;

public class CreateUserRequest : IRequest<Guid>
{
    public required User User { get; set; }
}
