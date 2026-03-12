using Desic.Infrastructure.Data.Providers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Desic.Infrastructure.Data.Providers;

public static class ConfigurationHelpers
{
    public static string GetConnectionString(this IConfiguration config, ConnectionStringType type)
    {
        var options = new ConnectionBehaviorOptions();
        var key = $"{ApplicationDatabaseConfigKeys.SqlServer.Section}:{type}:ConnectionBehavior";
        config.Bind(key, options);
        return GetConnectionStringFromBehavior(config, options);
    }

    private static string GetConnectionStringFromBehavior(this IConfiguration config, ConnectionBehaviorOptions options)
    {
        var connectionString = config.GetConnectionString(options.ConnectionStringName ?? throw new InvalidOperationException($"{nameof(options.ConnectionStringName)} is null for provided options"))
            ?? throw new InvalidOperationException($"Connection string is not configured: {options.ConnectionStringName}");
        var removeInitialCatalog = options.RemoveInitialCatalog ?? false;
        var replacePassword = options.ReplacePassword ?? false;
        var replaceUserId = options.ReplaceUserId ?? false;
        if (!replacePassword && !replaceUserId && !removeInitialCatalog) return connectionString;
        var builder = new SqlConnectionStringBuilder(connectionString);
        if (removeInitialCatalog) builder.InitialCatalog = string.Empty;
        if (replacePassword) builder.Password = options.Password;
        if (replaceUserId) builder.UserID = options.UserId;
        return builder.ConnectionString;
    }

    public enum ConnectionStringType
    {
        Api,
        Initialization,
        Migrations,
    }
}
