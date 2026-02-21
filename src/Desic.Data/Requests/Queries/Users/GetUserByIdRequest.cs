using Desic.Data.Entities;
using MediatR;

namespace Desic.Data.Requests.Queries.Users;

public class GetUserByIdRequest : IRequest<User?>
{
    public Guid UserId { get; set; }
}
