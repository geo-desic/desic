using Desic.Infrastructure.Data.Common.Models;
using MediatR;

namespace Desic.Infrastructure.Data.Test.Users;

public class SeedTestUsersRequest : SeedRequest, IRequest<SeedTestUsersResult>
{
    public int? Count { get; set; }
}
