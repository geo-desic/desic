using Desic.Application.Common;
using Desic.Application.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Application.Users.Get;

public sealed class GetUserByIdRequest : GetByIdRequest, IRequest<GetUserByIdRequest, Task<Result<User>>>
{
}
