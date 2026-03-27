using System.Text.Json.Serialization;

namespace Desic.Application.EntityTypes;

// entries alphabetized except potentially the first which should be the desired default value
[JsonConverter(typeof(JsonStringEnumConverter<EntityTypesOrderingMethod>))]
public enum EntityTypesOrderingMethod
{
    NameAsc, // default value
    KeyAsc,
    KeyDesc,
    NameDesc,
}
