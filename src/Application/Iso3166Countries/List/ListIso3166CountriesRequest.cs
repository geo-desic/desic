using Desic.Application.Common;
using Desic.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace Desic.Application.Iso3166Countries.List;

public class ListIso3166CountriesRequest : FilterableOrderableListRequest<Iso3166CountriesFilter, Iso3166CountriesOrderingMethod>, IRequest<Result<ListIso3166CountriesResult>>
{
    // overriding to specify desired default value
    [DefaultValue(Iso3166CountriesOrderingMethods.Default)]
    public override Iso3166CountriesOrderingMethod OrderingMethod { get; set; } = Iso3166CountriesOrderingMethods.Default;
}
