using Desic.Application.Common.Models;
using Desic.Domain.Iso3166Countries;
using System.Diagnostics.CodeAnalysis;

namespace Desic.Application.Iso3166Countries;

public class Iso3166CountryView : BaseModel, IIso3166Country
{
    public Iso3166CountryView() : base() { }

    [SetsRequiredMembers]
    public Iso3166CountryView(IReadOnlyGuidIdIso3166Country source) : base(source)
    {
        this.UpdateFrom(source);
    }

    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; } = null!;
    public required string Alpha3 { get; set; } = null!;
    public required string Name { get; set; } = null!;
}
