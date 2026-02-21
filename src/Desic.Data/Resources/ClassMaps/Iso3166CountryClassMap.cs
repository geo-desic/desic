using CsvHelper.Configuration;
using Desic.Data.Entities;

namespace Desic.Data.Resources.ClassMaps;

public class Iso3166CountryClassMap : ClassMap<Iso3166Country>
{
    public Iso3166CountryClassMap()
    {
        Map(m => m.IsoId).Name("id");
        Map(m => m.Alpha2).Name("alpha2");
        Map(m => m.Alpha3).Name("alpha3");
        Map(m => m.Name).Name("name");
    }
}
