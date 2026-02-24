using Desic.Core.Resources;
using MediatR;

namespace Desic.Core.Iso3166Countries;

public class Iso3166CountriesResourceStreamRequest : CsvResourceStreamRequest<Iso3166Country>, IStreamRequest<Iso3166Country> { }
