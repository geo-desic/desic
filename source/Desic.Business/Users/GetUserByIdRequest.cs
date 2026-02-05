using Desic.Business.Users.Models;
using FluentResults;
using MediatR;

namespace Desic.Business.Users
{
    public class GetUserByIdRequest : IRequest<Result<User>>
    {
        public Guid UserId { get; set; }
    }
}
