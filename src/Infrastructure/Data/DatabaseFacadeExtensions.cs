using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Desic.Infrastructure.Data;

internal static class DatabaseFacadeExtensions
{
    internal static string DateTimeUtc(this DatabaseFacade source)
    {
        return source.ProviderName switch
        {
            ProviderNames.Cosmos => "GetCurrentDateTime()",
            ProviderNames.MySql => "UTC_TIMESTAMP()",
            ProviderNames.MySqlPomelo => "UTC_TIMESTAMP()",
            ProviderNames.Oracle => "CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS DATE)",
            ProviderNames.PostgreSQL => "now() at time zone 'utc'",
            ProviderNames.Sqlite => "DATETIME('now')",
            ProviderNames.SqlServer => "SYSUTCDATETIME()",
            _ => "SYSUTCDATETIME()",
        };
    }

    private static bool IsSqlite(this DatabaseFacade source) => source.ProviderName == ProviderNames.Sqlite;

    internal static bool SupportsSchemas(this DatabaseFacade source) => !source.IsSqlite();
}
