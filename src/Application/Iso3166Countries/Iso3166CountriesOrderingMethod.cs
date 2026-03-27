using System.Text.Json.Serialization;

namespace Desic.Application.Iso3166Countries;

// entries alphabetized except potentially the first which should be the desired default value
[JsonConverter(typeof(JsonStringEnumConverter<Iso3166CountriesOrderingMethod>))]
public enum Iso3166CountriesOrderingMethod
{
    NameAsc, // default value
    Alpha2Asc,
    Alpha2Desc,
    Alpha3Asc,
    Alpha3Desc,
    IdAsc,
    IdDesc,
    IsoIdAsc,
    IsoIdDesc,
    NameDesc,
}
