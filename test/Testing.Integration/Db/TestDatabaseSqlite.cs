using Desic.Data;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseSqlite(string templateDatabaseFilePath) : ITestDatabase
{
    private string? _connectionString;
    private string? _databaseFilePath;
    private readonly string _templateDatabaseFilePath = templateDatabaseFilePath ?? throw new ArgumentNullException(nameof(templateDatabaseFilePath));

    public async ValueTask InitializeAsync()
    {
        var databaseDirectoryPath = Path.GetDirectoryName(_templateDatabaseFilePath)!;
        // create a unique name for the database
        var databaseFileName = $"{Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}.db"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFilePath = Path.Combine(databaseDirectoryPath, databaseFileName);

        File.Copy(_templateDatabaseFilePath, _databaseFilePath);

        _connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file after tests are done

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database");
    }

    public ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath)) TemplateDatabaseSqlite.DeleteDatabaseAndAssociatedFiles(_databaseFilePath);
        return ValueTask.CompletedTask;
    }

    public DbConnection GetConnection() => new SqliteConnection(_connectionString ?? throw NewDatabaseNotInitializedException());

    public string GetConnectionString() => _connectionString ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new(Constants.DatabaseNotInitialized);
}
