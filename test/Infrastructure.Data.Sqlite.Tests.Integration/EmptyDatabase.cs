using Desic.Testing.Integration;
using Desic.Testing.Integration.Db;
using Microsoft.Data.Sqlite;

namespace Desic.Infrastructure.Data.Sqlite.Tests.Integration;

public class EmptyDatabase : ITestDatabaseSqlite
{
    private readonly EmptyDatabaseSqlite _database = new(databaseDirectoryPath: Path.Combine(Path.GetTempPath(), Constants.TestsTempDirectoryName));

    public string DatabaseDirectoryPath => _database.DatabaseDirectoryPath;
    public string DatabaseFilePath => _database.DatabaseFilePath;
    public string DatabaseFileName => _database.DatabaseFileName;
    public string DatabaseName => _database.DatabaseName;
    public SqliteConnection GetSqliteConnection() => _database.GetSqliteConnection();
    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
    }
}
