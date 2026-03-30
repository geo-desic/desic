using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : FilterableOrderableListRequest<EntityTypesFilter, EntityTypesOrderingProperty>, IRequest<Result<ListEntityTypesResult>>
{
}
