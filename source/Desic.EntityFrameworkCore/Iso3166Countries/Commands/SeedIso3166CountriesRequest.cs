using Desic.EntityFrameworkCore.Iso3166Countries.Models;
using MediatR;

namespace Desic.EntityFrameworkCore.Iso3166Countries.Commands;

internal class SeedIso3166CountriesRequest : IRequest<SeedIso3166CountriesRequestHandlerResult>
{
    public int? BatchSize { get; set; }
}
