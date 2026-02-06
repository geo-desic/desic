using Desic.Business.Users.Models;
using Desic.Core.Mediator;
using FluentResults;

namespace Desic.Business.Users
{
    public class GetUserByIdRequest : IRequest<Result<User>>
    {
        public Guid UserId { get; set; }
    }
}
