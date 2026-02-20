using Microsoft.Extensions.Configuration;

namespace Desic.Testing.Integration.Core;

public static class TestConfiguration
{
    private static readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Test.json", optional: true)
        .AddUserSecrets("c121f405-33f5-48cb-b6f5-f4287fc75de8") // this should match the UserSecretsId in the test project .csproj file
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
