using Desic.Infrastructure.Data.Common.Models;
using MediatR;

namespace Desic.Infrastructure.Data.Tags;

public class SeedTagsRequest : SeedRequest, IRequest<SeedTagsResult>
{
}
