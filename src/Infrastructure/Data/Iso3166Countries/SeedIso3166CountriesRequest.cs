using Desic.Infrastructure.Data.Common.Models;
using MediatR;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public class SeedIso3166CountriesRequest : BatchesSeedRequest, IRequest<SeedResult>
{
}
