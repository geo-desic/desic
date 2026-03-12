using Microsoft.Extensions.Configuration;
using static Desic.Infrastructure.Data.Providers.ConfigurationHelpers;

namespace Desic.Infrastructure.Tools.DbUpdater;

public static class ConfigurationHelpers
{
    public static string GetConnectionStringInitialization(this IConfiguration config)
    {
        var result = config.GetValue<string>(ConfigKeys.ConnectionStringInitialization);
        if (result != null) return result;
        return config.GetConnectionString(ConnectionStringType.Initialization);
    }

    public static string GetConnectionStringMigrations(this IConfiguration config)
    {
        var result = config.GetValue<string>(ConfigKeys.ConnectionStringMigrations);
        if (result != null) return result;
        return config.GetConnectionString(ConnectionStringType.Migrations);
    }
}
