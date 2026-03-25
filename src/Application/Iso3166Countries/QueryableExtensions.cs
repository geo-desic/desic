namespace Desic.Application.Iso3166Countries;

public static class QueryableExtensions
{
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

    public static IOrderedQueryable<Domain.Iso3166Countries.Iso3166Country> OrderBy(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source, Iso3166CountriesOrderingMethod? orderingMethod)
    {
        return (orderingMethod ?? Iso3166CountriesOrderingMethod.NameAsc) switch
        {
            Iso3166CountriesOrderingMethod.Alpha2Asc => source.OrderBy(x => x.Alpha2),
            Iso3166CountriesOrderingMethod.Alpha2Desc => source.OrderByDescending(x => x.Alpha2),
            Iso3166CountriesOrderingMethod.Alpha3Asc => source.OrderBy(x => x.Alpha3),
            Iso3166CountriesOrderingMethod.Alpha3Desc => source.OrderByDescending(x => x.Alpha3),
            Iso3166CountriesOrderingMethod.IdAsc => source.OrderBy(x => x.Id),
            Iso3166CountriesOrderingMethod.IdDesc => source.OrderByDescending(x => x.Id),
            Iso3166CountriesOrderingMethod.IsoIdAsc => source.OrderBy(x => x.IsoId),
            Iso3166CountriesOrderingMethod.IsoIdDesc => source.OrderByDescending(x => x.IsoId),
            Iso3166CountriesOrderingMethod.NameAsc => source.OrderBy(x => x.Name),
            Iso3166CountriesOrderingMethod.NameDesc => source.OrderByDescending(x => x.Name),
            _ => source.OrderBy(x => x.Name),
        };
    }

    public static IQueryable<Iso3166Country> SelectToModel(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source) => source.Select(x => new Iso3166Country(x));

    public static IQueryable<Iso3166CountryView> SelectToView(this IQueryable<Domain.Iso3166Countries.Iso3166Country> source) => source.Select(x => new Iso3166CountryView(x));
}
