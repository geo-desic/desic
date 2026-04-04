using System.Text.Json.Serialization;

namespace Desic.Application.EntityTypes;

// entries alphabetized except potentially the first which should be the desired default value
// recommended: each entry name exactly matches the corresponding property name in the domain entity
[JsonConverter(typeof(JsonStringEnumConverter<EntityTypesOrderingProperty>))]
public enum EntityTypesOrderingProperty
{
    Name, // default value
    Key,
}
