using Desic.Core.Mediator;
using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Users.Queries
{
    public class GetUserByUsernameRequest : IRequest<User?>
    {
        public string? Username { get; set; }
    }
}
