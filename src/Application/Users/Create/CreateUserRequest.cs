using Desic.Application.Common;
using Desic.Application.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Application.Users.Create;

public sealed class CreateUserRequest : CreateRequest<CreateUser>, IRequest<CreateUserRequest, Task<Result<CreateUserResult>>>
{
}
