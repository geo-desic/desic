using Desic.Data.EntityTypes;
using Desic.Data.Shared.Entities;

namespace Desic.Data.Iso3166Countries;

public class Iso3166Country : SeedableSoftDeletableEntity, IIso3166CountryReferenceData
{
    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; }
    public required string Alpha3 { get; set; }
    public required string Name { get; set; }

    protected override SystemEntityType EnumEntityType => SystemEntityType.Iso3166Country;

    void IUpdatableFrom<IIso3166CountryReferenceData>.UpdateFrom(IIso3166CountryReferenceData from) => Iso3166CountryHelpers.UpdateFrom(this, from);

    bool IEquatable<IIso3166CountryReferenceData>.Equals(IIso3166CountryReferenceData? compare) => this.IsEquivalentTo(compare);
}
