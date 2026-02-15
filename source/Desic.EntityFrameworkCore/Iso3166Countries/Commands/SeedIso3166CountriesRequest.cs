using Desic.EntityFrameworkCore.Models;
using MediatR;

namespace Desic.EntityFrameworkCore.Iso3166Countries.Commands;

internal class SeedIso3166CountriesRequest : IRequest<DbSetSeedingResult>
{
    public int? BatchSize { get; set; }
}
