using Desic.Application.Common.Interfaces;

namespace Desic.Application.Iso3166Countries;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, Iso3166CountriesFilter filter) where T : Domain.Iso3166Countries.IReadOnlyIso3166Country, Domain.Common.Interfaces.IReadOnlyGuidId
    {
        var result = source;
        if (filter.Alpha2 != null) result = result.Where(x => x.Alpha2 == filter.Alpha2);
        if (filter.Alpha3 != null) result = result.Where(x => x.Alpha3 == filter.Alpha3);
        if (filter.Id != null) result = result.Where(x => x.Id == filter.Id);
        if (filter.IsoId != null) result = result.Where(x => x.IsoId == filter.IsoId);
        if (filter.Name != null) result = result.Where(x => x.Name == filter.Name);
        if (filter.NameContains != null) result = result.Where(x => x.Name.Contains(filter.NameContains));
        return result;
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IOrderingMethod<Iso3166CountriesOrderingProperty> orderingMethod) where T : Domain.Iso3166Countries.IReadOnlyIso3166Country, Domain.Common.Interfaces.IReadOnlyGuidId
    {
        return new Iso3166CountriesOrderer<T>().ApplyOrderingMethod(source, orderingMethod);
    }

    public static IQueryable<Iso3166Country> SelectToModel<T>(this IQueryable<T> source) where T : Domain.Iso3166Countries.IReadOnlyIso3166CountryEntity
        => source.Select(x => new Iso3166Country(x));

    public static IQueryable<Iso3166CountryView> SelectToView<T>(this IQueryable<T> source) where T : Domain.Iso3166Countries.IReadOnlyGuidIdIso3166Country
        => source.Select(x => new Iso3166CountryView(x));
}
