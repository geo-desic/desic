using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Iso3166Countries;

public class Iso3166Country : SeedableSoftDeletableEntity, IStaticEntityType, IIso3166CountryReferenceData
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Iso3166Country;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; }
    public required string Alpha3 { get; set; }
    public required string Name { get; set; }

    void IUpdatableFrom<IIso3166CountryReferenceData>.UpdateFrom(IIso3166CountryReferenceData from) => Iso3166CountryExtensions.UpdateFrom(this, from);

    bool IEquatable<IIso3166CountryReferenceData>.Equals(IIso3166CountryReferenceData? compare) => this.IsEquivalentTo(compare);
}
