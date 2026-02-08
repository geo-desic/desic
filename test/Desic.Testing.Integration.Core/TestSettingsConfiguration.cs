using Microsoft.Extensions.Configuration;

namespace Desic.Testing.Integration.Core;

public static class TestSettingsConfiguration
{
    private static readonly IConfiguration _configuration;

    public static IConfiguration Root => _configuration;

    static TestSettingsConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }
}
