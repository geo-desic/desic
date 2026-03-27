using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Models;
using System.ComponentModel;

namespace Desic.Application.Iso3166Countries;

public class Iso3166CountriesFilter : Filter
{
    [Description(Filters.DescriptionNonNullExact)]
    public string? Alpha2 { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public string? Alpha3 { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public Guid? Id { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public int? IsoId { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public string? Name { get; set; }

    [Description(Filters.DescriptionNonNullStringContains)]
    public string? NameContains { get; set; }
}
