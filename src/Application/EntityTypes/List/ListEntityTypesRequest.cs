using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : ListRequest, IRequest<Result<PaginatedList<EntityType>>>
{
}
