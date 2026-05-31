using Desic.Application.Common;
using Desic.Application.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Application.EntityTypes.List;

public sealed class ListEntityTypesRequest : FilterableOrderableListRequest<EntityTypesFilter, EntityTypesOrderingProperty>, IRequest<ListEntityTypesRequest, Task<Result<ListEntityTypesResult>>>
{
}
