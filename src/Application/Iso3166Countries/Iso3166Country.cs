using Desic.Application.Common.Models;
using Desic.Domain.Iso3166Countries;
using System.Diagnostics.CodeAnalysis;

namespace Desic.Application.Iso3166Countries;

public class Iso3166Country : SoftDeletableModel, IIso3166Country
{
    public Iso3166Country() : base() { }

    [SetsRequiredMembers]
    public Iso3166Country(Domain.Iso3166Countries.Iso3166Country source) : base(source)
    {
        this.UpdateFrom(source);
    }

    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; } = null!;
    public required string Alpha3 { get; set; } = null!;
    public required string Name { get; set; } = null!;
}
