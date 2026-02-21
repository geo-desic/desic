using Desic.Business.Models.Users;
using FluentResults;
using MediatR;

namespace Desic.Business.Requests.Queries.Users;

public class GetUserByIdRequest : IRequest<Result<User>>
{
    public Guid UserId { get; set; }
}
