using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlite(string templateDatabaseFilePath) : ITestDatabase
{
    private readonly EmptyDatabaseSqlite _database = new(databaseDirectoryPath: Path.GetDirectoryName(templateDatabaseFilePath) ?? throw new Exception($"Could not get directory from template database file path: {templateDatabaseFilePath}"));
    private readonly string _templateDatabaseFilePath = templateDatabaseFilePath ?? throw new ArgumentNullException(nameof(templateDatabaseFilePath));

    public DbConnection GetConnection() => _database.GetConnection();
    public string GetConnectionString() => _database.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        // copy the template database file to the new database file path before calling _database.InitializeAsync() which should use it
        File.Copy(_templateDatabaseFilePath, _database.DatabaseFilePath);

        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
    }
}
