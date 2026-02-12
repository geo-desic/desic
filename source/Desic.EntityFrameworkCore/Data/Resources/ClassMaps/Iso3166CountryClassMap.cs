using CsvHelper.Configuration;
using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Data.Resources.ClassMaps;

internal class Iso3166CountryClassMap : ClassMap<Iso3166Country>
{
    public Iso3166CountryClassMap()
    {
        Map(m => m.IsoId).Name("id");
        Map(m => m.Alpha2).Name("alpha2");
        Map(m => m.Alpha3).Name("alpha3");
        Map(m => m.Name).Name("name");
    }
}
