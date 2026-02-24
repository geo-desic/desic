using Desic.Core.Shared;
using MediatR;

namespace Desic.Core.Iso3166Countries;

public class SeedIso3166CountriesRequest : IRequest<EntitySetSeedingResult>
{
    public int? BatchSize { get; set; }
}
