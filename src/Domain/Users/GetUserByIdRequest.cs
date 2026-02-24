using MediatR;

namespace Desic.Domain.Users;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }
}
