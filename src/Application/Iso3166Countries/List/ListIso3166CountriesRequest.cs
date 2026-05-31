using Desic.Application.Common;
using Desic.Application.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Application.Iso3166Countries.List;

public sealed class ListIso3166CountriesRequest : FilterableOrderableListRequest<Iso3166CountriesFilter, Iso3166CountriesOrderingProperty>, IRequest<ListIso3166CountriesRequest, Task<Result<ListIso3166CountriesResult>>>
{
}
