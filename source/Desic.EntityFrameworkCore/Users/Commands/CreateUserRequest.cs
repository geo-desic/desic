using Desic.Core.Mediator;
using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Users.Commands
{
    public class CreateUserRequest : IRequest<Guid>
    {
        public required User User { get; set; }
    }
}
