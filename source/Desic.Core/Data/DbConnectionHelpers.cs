using System.Data.Common;

namespace Desic.Core.Data;

public static class DbConnectionHelpers
{
    public async static Task<bool> CanConnectAsync(this DbConnection connection, CancellationToken? cancellationToken = null)
    {
        cancellationToken ??= CancellationToken.None;
        try
        {
            await connection.OpenAsync(cancellationToken.Value);
            return true;
        }
        catch { /* nothing */ }
        return false;
    }

    public static bool CanConnect(this DbConnection connection)
    {
        try
        {
            connection.Open();
            return true;
        }
        catch { /* nothing */ }
        return false;
    }

    public static string? GetDatabaseName(this DbConnection connection)
    {
        try
        {
            return connection.Database;
        }
        catch { /* nothing */ }
        return null;
    }
}
