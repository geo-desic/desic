using Desic.Infrastructure.Data.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Infrastructure.Data.EntityTypes;

public sealed class SeedEntityTypesRequest : SeedRequest, IRequest<SeedEntityTypesRequest, Task<SeedEntityTypesResult>>
{
}
