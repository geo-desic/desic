using FluentResults;
using MediatR;

namespace Desic.Business.Users.Create;

public class CreateUserRequest : IRequest<Result<User>>
{
    public required UserCreate User { get; set; }
    public bool ReturnResult { get; set; }
}
