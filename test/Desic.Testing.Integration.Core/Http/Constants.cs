using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Testing.Integration.Core.Http;

public static class Constants
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
