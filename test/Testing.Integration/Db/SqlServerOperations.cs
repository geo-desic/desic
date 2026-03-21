using Microsoft.Data.SqlClient;
using System.Text;

namespace Desic.Testing.Integration.Db;

internal static class SqlServerOperations
{
    public static async Task BackupDatabase(string connectionString, string databaseBackupFilePath, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.BackupDatabase(databaseBackupFilePath: databaseBackupFilePath, databaseName: databaseName);
    }

    public static async Task BackupDatabase(this SqlConnection connection, string databaseBackupFilePath, string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"BACKUP DATABASE [{databaseName}] TO DISK = N'{databaseBackupFilePath}' WITH FORMAT, NAME = N'Full Backup of [{databaseName}]';";
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<SqlServerDatabaseFile>> BackupDatabaseReturningFileList(string connectionString, string databaseBackupFilePath, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.BackupDatabase(databaseBackupFilePath: databaseBackupFilePath, databaseName: databaseName);
        return await connection.FileListFromBackup(databaseBackupFilePath: databaseBackupFilePath);
    }

    public static async Task<List<SqlServerDatabaseFile>> BackupDatabaseReturningFileList(this SqlConnection connection, string databaseBackupFilePath, string databaseName)
    {
        await connection.BackupDatabase(databaseBackupFilePath: databaseBackupFilePath, databaseName: databaseName);
        return await connection.FileListFromBackup(databaseBackupFilePath: databaseBackupFilePath);
    }

    public static async Task<bool> ContainedDatabaseAuthenticationEnabled(this SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT [value_in_use] FROM sys.configurations WHERE name = 'contained database authentication';";
        var result = await command.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value && Convert.ToInt64(result) == 1)
        {
            return true;
        }
        return false;
    }

    public static async Task EnableContainedDatabaseAuthentication(this SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "EXEC sp_configure 'contained database authentication', 1;";
        await command.ExecuteNonQueryAsync();
        await connection.Reconfigure();
    }

    public static async Task<List<SqlServerDatabaseFile>> FileListFromBackup(this SqlConnection connection, string databaseBackupFilePath)
    {
        var result = new List<SqlServerDatabaseFile>();
        using var command = connection.CreateCommand();
        command.CommandText = $"RESTORE FILELISTONLY FROM DISK = '{databaseBackupFilePath}';";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new SqlServerDatabaseFile
            {
                LogicalName = DbValueToString(reader["LogicalName"]) ?? throw new NotSupportedException("Required backup file list value is null: LogicalName"),
                PhysicalName = DbValueToString(reader["PhysicalName"]) ?? throw new NotSupportedException("Required backup file list value is null: PhysicalName"),
                Type = DbValueToString(reader["Type"]) ?? throw new NotSupportedException("Required backup file list value is null: Type"),
            });
        }
        return result;
    }

    public static async Task CreateDatabase(string connectionString, bool contained, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.CreateDatabase(contained: contained, databaseName: databaseName);
    }

    public static async Task CreateDatabase(this SqlConnection connection, bool contained, string databaseName)
    {
        if (!await connection.ContainedDatabaseAuthenticationEnabled()) await connection.EnableContainedDatabaseAuthentication();
        var containmentType = contained ? "PARTIAL" : "NONE";
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE [{databaseName}] CONTAINMENT = {containmentType};";
        await command.ExecuteNonQueryAsync();
    }

    public static async Task DropDatabase(string connectionString, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.DropDatabase(databaseName: databaseName);
    }

    public static async Task DropDatabase(this SqlConnection connection, string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"IF DB_ID('{databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]; END";
        await command.ExecuteNonQueryAsync();
    }

    public static async Task RestoreDatabase(string connectionString, string databaseName, string backupFilePath, string backupDatabaseName, IEnumerable<SqlServerDatabaseFile> backupFileList)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var command = connection.CreateCommand();
        var builder = new StringBuilder();
        builder.Append($"RESTORE DATABASE [{databaseName}] FROM DISK = N'{backupFilePath}' WITH RECOVERY");
        foreach (var file in backupFileList)
        {
            var newFilePath = Path.Combine(Path.GetDirectoryName(file.PhysicalName)!, Path.GetFileName(file.PhysicalName).Replace(backupDatabaseName, databaseName));
            builder.Append($", MOVE '{file.LogicalName}' TO '{newFilePath}'");
        }
        builder.Append(';');
        command.CommandText = builder.ToString();
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<bool> IsContained(string connectionString, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return await connection.IsContained(databaseName);
    }

    public static async Task<bool> IsContained(this SqlConnection connection, string databaseName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT containment FROM sys.databases WHERE name = '{databaseName}';";
        var result = await command.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value && Convert.ToInt64(result) == 1)
        {
            return true;
        }
        return false;
    }

    public static async Task Reconfigure(this SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "RECONFIGURE;";
        await command.ExecuteNonQueryAsync();
    }

    private static string? DbValueToString(object? value)
    {
        if (value is null || value == DBNull.Value) return null;
        return value.ToString();
    }
}
