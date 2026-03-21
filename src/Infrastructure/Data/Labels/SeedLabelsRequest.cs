using Desic.Infrastructure.Data.Common.Models;
using MediatR;

namespace Desic.Infrastructure.Data.Labels;

public class SeedLabelsRequest : SeedRequest, IRequest<SeedLabelsResult>
{
}
