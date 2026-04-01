using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Iso3166Countries;

public class Iso3166Country : SeedableSoftDeletableEntity, IIso3166Country, IReadOnlyIso3166CountryEntity, IStaticEntityType, IReadOnlyNameable
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Iso3166Country;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; }
    public required string Alpha3 { get; set; }
    public required string Name { get; set; }

    public const int LengthAlpha2 = 2;
    public const int LengthAlpha3 = 3;
    public const int MaxLengthName = 100;
}
