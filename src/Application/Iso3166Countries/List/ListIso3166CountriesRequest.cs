using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;

namespace Desic.Application.Iso3166Countries.List;

public class ListIso3166CountriesRequest : FilterableOrderableListRequest<Iso3166CountriesFilter, Iso3166CountriesOrderingProperty>, IRequest<Result<ListIso3166CountriesResult>>
{
}
