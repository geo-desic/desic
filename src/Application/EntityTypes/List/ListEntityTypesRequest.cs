using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequest : FilterableOrderableListRequest<EntityTypesFilter, EntityTypesOrderingMethod>, IRequest<Result<ListEntityTypesResult>>
{
    // overriding to specify desired default value
    [DefaultValue(EntityTypesOrderingMethods.Default)]
    public override EntityTypesOrderingMethod OrderingMethod { get; set; } = EntityTypesOrderingMethods.Default;
}
