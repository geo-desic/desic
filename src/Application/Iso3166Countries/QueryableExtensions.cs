using Desic.Application.Common.Interfaces;

namespace Desic.Application.Iso3166Countries;

public static class QueryableExtensions
{
    private static readonly Lazy<Iso3166CountriesOrderer> _orderer = new();

    public static IQueryable<Domain.Iso3166Countries.Iso3166Country> ApplyFilter(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source, Iso3166CountriesFilter filter)
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

    public static IOrderedQueryable<Domain.Iso3166Countries.Iso3166Country> OrderBy(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source, IOrderingMethod<Iso3166CountriesOrderingProperty> orderingMethod)
    {
        return _orderer.Value.ApplyOrderingMethod(source, orderingMethod);
    }

    public static IQueryable<Iso3166Country> SelectToModel(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source) => source.Select(x => new Iso3166Country(x));

    public static IQueryable<Iso3166CountryView> SelectToView(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source) => source.Select(x => new Iso3166CountryView(x));
}
