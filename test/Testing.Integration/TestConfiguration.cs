using Microsoft.Extensions.Configuration;

namespace Desic.Testing.Integration;

public static class TestConfiguration
{
    private static readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Test.json", optional: true)
        .AddUserSecrets<IMarker>(optional: true)
        .AddEnvironmentVariables()
        .Build();

    private static readonly IntegrationTestsOptions _options = new();

    static TestConfiguration()
    {
        _configuration.Bind(_options);
    }

    public static IntegrationTestsOptions Options => _options;

    public static IConfigurationRoot Root => _configuration;
}
