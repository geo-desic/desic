using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : FilterableOrderableListRequest<ListEntityTypesFilters, EntityTypesOrderingMethod>, IRequest<Result<ListEntityTypesResult>>
{
    // overriding to specify desired default value
    [DefaultValue(EntityTypesOrderingMethod.NameAsc)]
    public override EntityTypesOrderingMethod OrderingMethod { get; set; } = EntityTypesOrderingMethod.NameAsc;
}
