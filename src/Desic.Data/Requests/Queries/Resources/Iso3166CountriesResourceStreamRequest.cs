using Desic.Data.Entities;
using MediatR;

namespace Desic.Data.Requests.Queries.Resources;

public class Iso3166CountriesResourceStreamRequest : CsvResourceStreamRequest<Iso3166Country>, IStreamRequest<Iso3166Country> { }
