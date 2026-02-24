using MediatR;

namespace Desic.Domain.Users;

public class CreateUserRequest : IRequest<Guid>
{
    public required User User { get; set; }
}
