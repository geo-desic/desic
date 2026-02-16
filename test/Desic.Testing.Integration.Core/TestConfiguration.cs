using Microsoft.Extensions.Configuration;

namespace Desic.Testing.Integration.Core;

public static class TestConfiguration
{
    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Test.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    private static readonly IntegrationTestsOptions _options = new();

    static TestConfiguration()
    {
        _configuration.Bind(_options);
    }

    public static IntegrationTestsOptions Options => _options;

    public static IConfiguration Root => _configuration;
}
