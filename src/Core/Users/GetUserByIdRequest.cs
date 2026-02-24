using MediatR;

namespace Desic.Core.Users;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }
}
