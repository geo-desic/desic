using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.Users.Create;

public class CreateUserRequest : CreateRequest<UserCreate>, IRequest<Result<CreateResult<User>>>
{
}
