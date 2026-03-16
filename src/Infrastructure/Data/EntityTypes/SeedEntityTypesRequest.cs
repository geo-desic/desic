using Desic.Infrastructure.Data.Common.Models;
using MediatR;

namespace Desic.Infrastructure.Data.EntityTypes;

public class SeedEntityTypesRequest : SeedRequest, IRequest<SeedEntityTypesResult>
{
}
