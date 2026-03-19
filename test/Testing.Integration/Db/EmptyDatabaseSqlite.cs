using Desic.Shared.Data;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

public sealed class EmptyDatabaseSqlite(string databaseDirectoryPath) : ITestDatabase
{
    private string? _connectionString;
    private string? _databaseFilePath;
    private string? _databaseFileName;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private readonly string _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes

    public DbConnection GetConnection() => new SqliteConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseFilePath => _databaseFilePath ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseFileName => _databaseFileName ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseName => _databaseName;

    public async ValueTask InitializeAsync()
    {
        _databaseFileName = $"{_databaseName}.db";
        _databaseFilePath = Path.Combine(_databaseDirectoryPath, _databaseFileName);

        _connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file(s) after tests are done

        using var connection = new SqliteConnection(_connectionString);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database"); // opening this connection should create the database file
        await connection.CloseAsync();
    }

    public ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath)) SqliteOperations.DeleteDatabaseAndAssociatedFiles(_databaseFilePath);
        return ValueTask.CompletedTask;
    }
}
