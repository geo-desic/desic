using Desic.Application.Common.Models;

namespace Desic.Infrastructure.Data.Common.Models;

public class SeedRequest : ByRequest
{
    public SeedApplicationDatabaseMethod Method { get; set; } = SeedApplicationDatabaseRequestHandler.DefaultSeedingMethod;
}
