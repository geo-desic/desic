using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simpler IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlServerLocal(SeededAppDatabaseTemplateSqlServerLocal databaseTemplate) : IDatabase
{
    private string? _connectionString;
    private readonly EmptyDatabaseSqlServerLocal _database = new(connectionString: databaseTemplate.ConnectionStringInitialization, contained: databaseTemplate.IsContained,
        databaseCreator: async (connectionString, databaseName, contained) => await SqlServerOperations.RestoreDatabase(connectionString: connectionString, databaseName: databaseName, backupFilePath: databaseTemplate.BackupFilePath,
            backupDatabaseName: databaseTemplate.DatabaseName, backupFileList: databaseTemplate.GetBackupFileList()));
    private readonly SeededAppDatabaseTemplateSqlServerLocal _databaseTemplate = databaseTemplate ?? throw new ArgumentNullException(nameof(databaseTemplate));

    public string DatabaseName => _database.DatabaseName;
    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        // this restores the database backup to the new database name using the databaseCreator logic above
        await _database.InitializeAsync();

        await SetUserLogins();

        _connectionString = new SqlConnectionStringBuilder(_databaseTemplate.ConnectionStringApi) { InitialCatalog = _database.DatabaseName }.ConnectionString;

        using var connection = GetConnection();
        await connection.OpenAsync();
    }

    private async Task SetUserLogins()
    {
        if (_databaseTemplate.IsContained) return;
        var connectionsString = new SqlConnectionStringBuilder(_databaseTemplate.ConnectionStringInitialization) { InitialCatalog = _database.DatabaseName }.ConnectionString;
        using var connection = new SqlConnection(connectionsString);
        await connection.OpenAsync();
        foreach (var user in _databaseTemplate.UsersOptions.Values)
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
