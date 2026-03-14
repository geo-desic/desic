using System.Data.Common;

namespace Desic.Data;

public static class DbConnectionExtensions
{
    public async static Task<bool> TryOpenAsync(this DbConnection connection, CancellationToken? cancellationToken = null)
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
}
