using FluentResults;
using MediatR;

namespace Desic.Application.Users.Get;

public class GetUserByIdRequest : IRequest<Result<User>>
{
    public Guid UserId { get; set; }
}
