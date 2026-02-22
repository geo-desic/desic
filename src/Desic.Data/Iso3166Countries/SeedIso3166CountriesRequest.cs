using Desic.Data.Shared;
using MediatR;

namespace Desic.Data.Iso3166Countries;

public class SeedIso3166CountriesRequest : IRequest<EntitySetSeedingResult>
{
    public int? BatchSize { get; set; }
}
