using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Testing.Integration.Http;

public static class Constants
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
