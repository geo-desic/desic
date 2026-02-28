using Desic.Domain.Iso3166Countries;
using Desic.Infrastructure.Data.Resources;
using MediatR;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public class Iso3166CountriesResourceStreamRequest : CsvResourceStreamRequest<Iso3166Country>, IStreamRequest<Iso3166Country> { }
