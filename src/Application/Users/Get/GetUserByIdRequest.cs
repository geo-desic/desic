using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.Users.Get;

public class GetUserByIdRequest : GetByIdRequest, IRequest<Result<User>>
{
}
