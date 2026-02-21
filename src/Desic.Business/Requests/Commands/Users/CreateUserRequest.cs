using Desic.Business.Models.Users;
using FluentResults;
using MediatR;

namespace Desic.Business.Requests.Commands.Users;

public class CreateUserRequest : IRequest<Result<User>>
{
    public required UserCreate User { get; set; }
    public bool ReturnResult { get; set; }
}
