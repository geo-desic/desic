using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

public sealed class SqlServerDatabaseOnlineWaitStrategy(string databaseName) : IWaitUntil
{
    private readonly string _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));

    public async Task<bool> UntilAsync(IContainer container)
    {
        var msSqlContainer = container as MsSqlContainer ?? throw new NotSupportedException("Container is not an MsSqlContainer");
        try
        {
            using var connection = new SqlConnection(msSqlContainer.GetConnectionString());
            await connection.OpenAsync().ConfigureAwait(false);
            using var command = connection.CreateCommand();
            {
                command.CommandText = $"SELECT 1 FROM [sys].[databases] WHERE [state] = 0 AND [name] = '{_databaseName}'"; // [state] = 0 means ONLINE
                var result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value && Convert.ToInt32(result) == 1) return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return false;
    }
}
