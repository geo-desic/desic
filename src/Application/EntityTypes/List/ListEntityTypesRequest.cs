using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : IRequest<Result<PaginatedList<EntityType>>>
{
    public int? Count { get; set; }
    public int StartIndex { get; set; }
}
