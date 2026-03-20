using Desic.Shared.Data;
using Microsoft.Data.Sqlite;

namespace Desic.Testing.Integration.Db;

public sealed class EmptyDatabaseSqlite : ITestDatabaseSqlite
{
    private string? _connectionString;
    private readonly string _databaseFilePath;
    private readonly string _databaseFileName;
    private readonly string _databaseDirectoryPath;
    private readonly string _databaseName;

    public EmptyDatabaseSqlite(string databaseDirectoryPath, string? databaseNamePrefix = null)
    {
        _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
        _databaseName = $"{databaseNamePrefix ?? Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFileName = $"{_databaseName}.db";
        _databaseFilePath = Path.Combine(_databaseDirectoryPath, _databaseFileName);
    }

    public string DatabaseDirectoryPath => _databaseDirectoryPath;
    public string DatabaseFilePath => _databaseFilePath;
    public string DatabaseFileName => _databaseFileName;
    public string DatabaseName => _databaseName;
    public SqliteConnection GetSqliteConnection() => new(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        _connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file(s) after tests are done

        using var connection = GetSqliteConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database"); // opening this connection should create the database file
    }

    public ValueTask DisposeAsync()
    {
        SqliteOperations.DeleteDatabaseAndAssociatedFiles(_databaseFilePath);
        return ValueTask.CompletedTask;
    }
}
