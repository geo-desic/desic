using Desic.Core.Mediator;
using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Users.Queries
{
    public class GetUserByIdRequest : IRequest<User?>
    {
        public Guid UserId { get; set; }
    }
}
