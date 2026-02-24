using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Desic.Infrastructure;

internal static class DatabaseFacadeExtensions
{
    internal static string DateTimeUtc(this DatabaseFacade db)
    {
        return db.ProviderName switch
        {
            ProviderNames.Cosmos => "GetCurrentDateTime()",
            ProviderNames.MySql => "UTC_TIMESTAMP()",
            ProviderNames.MySqlPomelo => "UTC_TIMESTAMP()",
            ProviderNames.Oracle => "CAST(SYS_EXTRACT_UTC(SYSTIMESTAMP) AS DATE)",
            ProviderNames.PostgreSQL => "now() at time zone 'utc'",
            ProviderNames.Sqlite => "DATETIME('now')",
            ProviderNames.SqlServer => "GETUTCDATE()",
            _ => "GETUTCDATE()",
        };
    }
}
