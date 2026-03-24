using System.Text.Json.Serialization;

namespace Desic.Application.EntityTypes;

[JsonConverter(typeof(JsonStringEnumConverter<EntityTypesOrderingMethod>))]
public enum EntityTypesOrderingMethod
{
    KeyAsc,
    KeyDesc,
    NameAsc,
    NameDesc,
}
