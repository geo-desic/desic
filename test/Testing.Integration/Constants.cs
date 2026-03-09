using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Testing.Integration;

public static class Constants
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public const string DatabaseName = "Desic";

    public const string DatabaseNotInitialized = "Database has not been initialized";

    public const string TestEnvironmentName = "Test";
}
