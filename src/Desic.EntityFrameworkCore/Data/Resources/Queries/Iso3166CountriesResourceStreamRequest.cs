using Desic.EntityFrameworkCore.Entities;
using MediatR;

namespace Desic.EntityFrameworkCore.Data.Resources.Queries;

public class Iso3166CountriesResourceStreamRequest : CsvResourceStreamRequest<Iso3166Country>, IStreamRequest<Iso3166Country> { }
