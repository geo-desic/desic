using Desic.Infrastructure.Data.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Infrastructure.Data.Test.Users;

public sealed class SeedTestUsersRequest : SeedRequest, IRequest<SeedTestUsersRequest, Task<SeedTestUsersResult>>
{
    public int? Count { get; set; }
}
