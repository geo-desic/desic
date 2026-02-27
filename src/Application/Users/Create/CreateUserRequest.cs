using Desic.Application.Common;
using MediatR;

namespace Desic.Application.Users.Create;

public class CreateUserRequest : IRequest<Result<CreateResult<User>>>
{
    public required UserCreate User { get; set; }
    public bool ReturnRepresentation { get; set; }
}
