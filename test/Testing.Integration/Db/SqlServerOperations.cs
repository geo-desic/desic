using Microsoft.Data.SqlClient;

namespace Desic.Testing.Integration.Db;

internal static class SqlServerOperations
{
    public static async Task BackupDatabase(string connectionString, string databaseBackupFilePath, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"BACKUP DATABASE [{databaseName}] TO DISK = N'{databaseBackupFilePath}' WITH FORMAT, NAME = N'Full Backup of [{databaseName}]';";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    public static async Task CreateDatabase(string connectionString, bool contained, string databaseName)
    {
        var containmentType = contained ? "PARTIAL" : "NONE";
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"CREATE DATABASE [{databaseName}] CONTAINMENT = {containmentType};";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    public static async Task DropDatabase(string connectionString, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"IF DB_ID('{databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]; END";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    public static async Task RestoreDatabase(string connectionString, string databaseName, string backupFilePath, string backupLogicalNameData, string backupLogicalNameLog, string databaseFilePath, string databaseLogFilePath)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"RESTORE DATABASE [{databaseName}] FROM DISK = N'{backupFilePath}' WITH MOVE '{backupLogicalNameData}' TO '{databaseFilePath}', MOVE '{backupLogicalNameLog}' TO '{databaseLogFilePath}', RECOVERY;";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    public static async Task<bool> IsContained(this SqlConnection connection, string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT containment FROM sys.databases WHERE name = '{databaseName}';";
        var result = await command.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value && Convert.ToInt64(result) == 1) // non-contained database
        {
            return true;
        }
        return false;
    }
}
