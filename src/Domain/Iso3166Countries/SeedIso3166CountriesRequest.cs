using Desic.Domain.Common;
using MediatR;

namespace Desic.Domain.Iso3166Countries;

public class SeedIso3166CountriesRequest : IRequest<EntitySetSeedingResult>
{
    public int? BatchSize { get; set; }
}
