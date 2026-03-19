using Desic.Shared.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simpler IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlServerLocal(SeededAppTemplateDatabaseSqlServerLocal templateDatabase) : ITestDatabase
{
    private string? _connectionString;
    private string? _databaseFilePath;
    private string? _databaseLogFilePath;
    private string? _databaseName;
    private readonly SeededAppTemplateDatabaseSqlServerLocal _templateDatabase = templateDatabase ?? throw new ArgumentNullException(nameof(templateDatabase));

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFilePath = Path.Combine(_templateDatabase.DirectoryPath, $"{_databaseName}.mdf");
        _databaseLogFilePath = Path.Combine(_templateDatabase.DirectoryPath, $"{_databaseName}.ldf");

        await SqlServerOperations.RestoreDatabase(connectionString: _templateDatabase.ConnectionStringInitialization, databaseName: _databaseName, backupFilePath: _templateDatabase.BackupFilePath,
            backupLogicalNameData: _templateDatabase.Name, backupLogicalNameLog: $"{_templateDatabase.Name}_Log", databaseFilePath: _databaseFilePath, databaseLogFilePath: _databaseLogFilePath);
        Console.Write($"Successfully restored database [{_databaseName}] using '{_templateDatabase.BackupFilePath}'");

        await SetUserLogins();

        _connectionString = new SqlConnectionStringBuilder(_templateDatabase.ConnectionStringApi) { InitialCatalog = _databaseName }.ConnectionString;

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
    }

    private async Task SetUserLogins()
    {
        var connectionsString = new SqlConnectionStringBuilder(_templateDatabase.ConnectionStringInitialization) { InitialCatalog = _databaseName }.ConnectionString;
        using var connection = new SqlConnection(connectionsString);
        await connection.OpenAsync();
        if (!await connection.IsContained(databaseName: _databaseName!))
        {
            foreach (var user in _templateDatabase.UsersOptions.Values)
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"ALTER USER [{user.Name}] WITH LOGIN = [{user.LoginName}];";
                await command.ExecuteNonQueryAsync();
            }
            Console.Write($"Successfully set logins for database users");
        }
        await connection.CloseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseName)) await SqlServerOperations.DropDatabase(connectionString: _templateDatabase.ConnectionStringInitialization, databaseName: _databaseName);
        if (!string.IsNullOrEmpty(_databaseFilePath)) try { File.Delete(_databaseFilePath); } catch { /* nothing */ }
        if (!string.IsNullOrEmpty(_databaseLogFilePath)) try { File.Delete(_databaseLogFilePath); } catch { /* nothing */ }
    }
}
