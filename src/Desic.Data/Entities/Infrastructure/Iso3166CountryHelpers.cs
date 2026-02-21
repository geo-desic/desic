using Desic.Helpers;

namespace Desic.Data.Entities.Infrastructure;

public static class Iso3166CountryHelpers
{
    public static bool IsEquivalentTo(this IIso3166CountryReferenceData? item1, IIso3166CountryReferenceData? item2)
    {
        if (item1 == null || item2 == null) return item1.NullablyEquivalentTo(item2);
        return item1.IsoId == item2.IsoId && item1.Alpha2 == item2.Alpha2 && item1.Alpha3 == item2.Alpha3 && item1.Name == item2.Name;
    }

    public static void UpdateFrom(this IIso3166CountryReferenceData update, IIso3166CountryReferenceData from)
    {
        update.IsoId = from.IsoId;
        update.Alpha2 = from.Alpha2;
        update.Alpha3 = from.Alpha3;
        update.Name = from.Name;
    }
}
