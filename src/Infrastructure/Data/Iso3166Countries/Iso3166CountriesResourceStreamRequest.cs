using Desic.Domain.Iso3166Countries;
using Desic.Infrastructure.Data.Resources;
using DispatchR.Abstractions.Stream;

namespace Desic.Infrastructure.Data.Iso3166Countries;

public sealed class Iso3166CountriesResourceStreamRequest : CsvResourceStreamRequest<Iso3166Country>, IStreamRequest<Iso3166CountriesResourceStreamRequest, Iso3166Country> { }
