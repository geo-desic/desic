using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : IRequest<Result<PaginatedList<EntityType>>>
{
    public int Offset { get; set; } = 0;
}
