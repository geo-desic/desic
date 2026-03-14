using Desic.Infrastructure.Data.Providers;
using Microsoft.Extensions.Configuration;

namespace Desic.Infrastructure.Tools.DbUpdater;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringInitialization(this IConfiguration config)
    {
        var result = config.GetValue<string>(ConfigKeys.ConnectionStringInitialization);
        if (result != null) return result;
        return config.GetSqlServerConnectionString(ConnectionStringType.Initialization);
    }

    public static string GetConnectionStringMigrations(this IConfiguration config)
    {
        var result = config.GetValue<string>(ConfigKeys.ConnectionStringMigrations);
        if (result != null) return result;
        return config.GetSqlServerConnectionString(ConnectionStringType.Migrations);
    }
}
