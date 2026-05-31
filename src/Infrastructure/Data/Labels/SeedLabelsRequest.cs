using Desic.Infrastructure.Data.Common.Models;
using DispatchR.Abstractions.Send;
namespace Desic.Infrastructure.Data.Labels;

public sealed class SeedLabelsRequest : SeedRequest, IRequest<SeedLabelsRequest, Task<SeedLabelsResult>>
{
}
