using Desic.Data.Models;
using MediatR;

namespace Desic.Data.Requests.Commands.Iso3166Countries;

public class SeedIso3166CountriesRequest : IRequest<EntitySetSeedingResult>
{
    public int? BatchSize { get; set; }
}
