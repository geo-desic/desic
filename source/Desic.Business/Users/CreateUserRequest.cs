using Desic.Business.Users.Models;
using Desic.Core.Mediator;
using FluentResults;

namespace Desic.Business.Users
{
    public class CreateUserRequest : IRequest<Result<User>>
    {
        public required UserCreate User { get; set; }
        public bool ReturnResult { get; set; }
    }
}
