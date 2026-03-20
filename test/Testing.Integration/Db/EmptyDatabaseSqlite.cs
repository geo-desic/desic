using Microsoft.Data.Sqlite;

namespace Desic.Testing.Integration.Db;

public sealed class EmptyDatabaseSqlite : IDatabaseSqlite
{
    private string? _connectionString;
    private readonly string _databaseFilePath;
    private readonly string _databaseFileName;
    private readonly string _databaseDirectoryPath;
    private readonly string _databaseName;

    public EmptyDatabaseSqlite(string? databaseDirectoryPath = null, string? databaseNamePrefix = null)
    {
        _databaseDirectoryPath = databaseDirectoryPath ?? Path.Combine(Path.GetTempPath(), Constants.TestsTempDirectoryName);
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
        Directory.CreateDirectory(_databaseDirectoryPath);
        _connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file(s) after tests are done

        using var connection = GetSqliteConnection();
        await connection.OpenAsync();
    }

    public ValueTask DisposeAsync()
    {
        SqliteOperations.DeleteDatabaseAndAssociatedFiles(_databaseFilePath);
        return ValueTask.CompletedTask;
    }
}
