using Desic.Business.Users.Models;
using MediatR;

namespace Desic.Business.Users
{
    public class GetUserByIdRequest : IRequest<User?>
    {
        public Guid UserId { get; set; }
    }
}
