using FluentResults;
using MediatR;

namespace Desic.Business.Users.Get;

public class GetUserByIdRequest : IRequest<Result<User>>
{
    public Guid UserId { get; set; }
}
