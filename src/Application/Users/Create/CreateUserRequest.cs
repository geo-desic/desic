using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.Users.Create;

public class CreateUserRequest : CreateRequest<CreateUser>, IRequest<Result<CreateUserResult>>
{
}
