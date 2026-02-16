using Microsoft.Extensions.Configuration;

namespace Desic.Testing.Integration.Core;

public static class TestSettingsConfiguration
{
    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("appsettings.Test.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    public static IConfiguration Root => _configuration;
}
