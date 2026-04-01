using Desic.Application.Common.Infrastructure;

namespace Desic.Application.Iso3166Countries;

internal sealed class Iso3166CountriesOrderer<T> : EnumQueryableOrderer<Iso3166CountriesOrderingProperty, T> where T : Domain.Iso3166Countries.IReadOnlyIso3166Country, Domain.Common.Interfaces.IReadOnlyGuidId
{
    public Iso3166CountriesOrderer()
    {
        Map(Iso3166CountriesOrderingProperty.Alpha2, x => x.Alpha2);
        Map(Iso3166CountriesOrderingProperty.Alpha3, x => x.Alpha3);
        Map(Iso3166CountriesOrderingProperty.Id, x => x.Id);
        Map(Iso3166CountriesOrderingProperty.IsoId, x => x.IsoId);
        Map(Iso3166CountriesOrderingProperty.Name, x => x.Name);
    }
}
