using Desic.Infrastructure.Data.Providers;

namespace Desic.AppHost;

public static class ConfigKeys
{
    public const string PasswordInitialization = $"{ApplicationDatabaseConfigKeys.SqlServer.SectionInitializationConnectionBehavior}:Password";
    public const string PasswordMigrations = $"{ApplicationDatabaseConfigKeys.SqlServer.SectionMigrationsConnectionBehavior}:Password";
    public const string PasswordApi = $"{ApplicationDatabaseConfigKeys.SqlServer.SectionApiConnectionBehavior}:Password";
}
