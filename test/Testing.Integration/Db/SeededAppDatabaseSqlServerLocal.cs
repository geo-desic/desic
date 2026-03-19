using Desic.Shared.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simpler IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlServerLocal(SeededAppTemplateDatabaseSqlServerLocal templateDatabase) : ITestDatabase
{
    private string? _connectionString;
    private readonly EmptyDatabaseSqlServerLocal _database = new(connectionString: templateDatabase.ConnectionStringInitialization, contained: templateDatabase.IsContained,
        databaseCreator: async (connectionString, databaseName, contained) => await SqlServerOperations.RestoreDatabase(connectionString: connectionString, databaseName: databaseName, backupFilePath: templateDatabase.BackupFilePath,
            backupDatabaseName: templateDatabase.DatabaseName, backupFileList: templateDatabase.GetBackupFileList()));
    private readonly SeededAppTemplateDatabaseSqlServerLocal _templateDatabase = templateDatabase ?? throw new ArgumentNullException(nameof(templateDatabase));

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        // this restores the database backup to the new database name using the databaseCreator logic above
        await _database.InitializeAsync();

        await SetUserLogins();

        _connectionString = new SqlConnectionStringBuilder(_templateDatabase.ConnectionStringApi) { InitialCatalog = _database.DatabaseName }.ConnectionString;

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
    }

    private async Task SetUserLogins()
    {
        if (_templateDatabase.IsContained) return;
        var connectionsString = new SqlConnectionStringBuilder(_templateDatabase.ConnectionStringInitialization) { InitialCatalog = _database.DatabaseName }.ConnectionString;
        using var connection = new SqlConnection(connectionsString);
        await connection.OpenAsync();
        foreach (var user in _templateDatabase.UsersOptions.Values)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER USER [{user.Name}] WITH LOGIN = [{user.LoginName}];";
            await command.ExecuteNonQueryAsync();
        }
        Console.Write($"Successfully set logins for database users");
        await connection.CloseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
    }
}
