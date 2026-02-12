using Desic.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Data.Resources.Queries;

public class Iso3166CountriesResourceStreamRequestHandler(ILogger<CsvResourceStreamRequestHandler<Iso3166Country>> logger) : CsvResourceStreamRequestHandler<Iso3166Country>(logger), IStreamRequestHandler<Iso3166CountriesResourceStreamRequest, Iso3166Country>
{
    public IAsyncEnumerable<Iso3166Country> Handle(Iso3166CountriesResourceStreamRequest request, CancellationToken cancellationToken)
    {
        return base.Handle(request, cancellationToken);
    }
}
