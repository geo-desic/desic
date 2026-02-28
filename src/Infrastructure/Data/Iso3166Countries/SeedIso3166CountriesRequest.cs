using Desic.Infrastructure.Data.Common;
using MediatR;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public class SeedIso3166CountriesRequest : IRequest<EntitySetSeedingResult>
{
    public int? BatchSize { get; set; }
}
