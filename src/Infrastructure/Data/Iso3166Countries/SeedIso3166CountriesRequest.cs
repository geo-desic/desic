using Desic.Infrastructure.Data.Common.Models;
using DispatchR.Abstractions.Send;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public sealed class SeedIso3166CountriesRequest : BatchesSeedRequest, IRequest<SeedIso3166CountriesRequest, Task<SeedIso3166CountriesResult>>
{
}
