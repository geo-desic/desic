using MediatR;

namespace Desic.Core.Users;

public class CreateUserRequest : IRequest<Guid>
{
    public required User User { get; set; }
}
