using Desic.Data.Shared.Entities;

namespace Desic.Data.Iso3166Countries;

public interface IIso3166CountryReferenceData : IEquatable<IIso3166CountryReferenceData>, IUpdatableFrom<IIso3166CountryReferenceData>
{
    int IsoId { get; set; }
    string Alpha2 { get; set; }
    string Alpha3 { get; set; }
    string Name { get; set; }
}
