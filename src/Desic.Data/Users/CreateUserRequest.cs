using MediatR;

namespace Desic.Data.Users;

public class CreateUserRequest : IRequest<Guid>
{
    public required User User { get; set; }
}
