using System.Text.Json.Serialization;

namespace Desic.Application.Iso3166Countries;

[JsonConverter(typeof(JsonStringEnumConverter<Iso3166CountriesOrderingMethod>))]
public enum Iso3166CountriesOrderingMethod
{
    Alpha2Asc,
    Alpha2Desc,
    Alpha3Asc,
    Alpha3Desc,
    IdAsc,
    IdDesc,
    IsoIdAsc,
    IsoIdDesc,
    NameAsc,
    NameDesc,
}
