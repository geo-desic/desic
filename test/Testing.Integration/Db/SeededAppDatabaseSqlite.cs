using Microsoft.Data.Sqlite;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlite(string databaseTemplateFilePath) : IDatabaseSqlite
{
    private readonly EmptyDatabaseSqlite _database = new(databaseDirectoryPath: Path.GetDirectoryName(databaseTemplateFilePath) ?? throw new Exception($"Could not get directory from database template file path: {databaseTemplateFilePath}"));
    private readonly string _databaseTemplateFilePath = databaseTemplateFilePath ?? throw new ArgumentNullException(nameof(databaseTemplateFilePath));

    public string DatabaseDirectoryPath => _database.DatabaseDirectoryPath;
    public string DatabaseFilePath => _database.DatabaseFilePath;
    public string DatabaseFileName => _database.DatabaseFileName;
    public string DatabaseName => _database.DatabaseName;
    public SqliteConnection GetSqliteConnection() => _database.GetSqliteConnection();
    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        // copy the template database file to the new database file path before calling _database.InitializeAsync() which should use it
        File.Copy(_databaseTemplateFilePath, _database.DatabaseFilePath);

        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
    }
}
