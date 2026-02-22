using MediatR;

namespace Desic.Data.Users;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }
}
