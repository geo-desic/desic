using Desic.Data;
using Desic.Infrastructure.Data.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseSqlite : ITestDatabase
{
    private string? _connectionString;
    private string? _databaseFilePath;

    public async ValueTask InitializeAsync()
    {
        // make sure temporary directory for the database files exists
        var tempDir = Path.Combine(Path.GetTempPath(), "desic-tests");
        Directory.CreateDirectory(tempDir);

        // create a unique name for the database
        var databaseFileName = $"desic_{Guid.CreateVersion7():N}.db"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFilePath = Path.Combine(tempDir, databaseFileName);

        _connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file after tests are done

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", _connectionString, "--environment", Constants.TestEnvironmentName]);

        await context.Database.MigrateAsync();

        using var connection = GetConnection();
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public ValueTask DisposeAsync()
    {
        // delete the primary database file
        if (_databaseFilePath != null) File.Delete(_databaseFilePath);

        // delete any associated files that sqlite automatically creates
        string[] associatedExtensions = ["db-journal", "db-wal", "db-shm"];
        foreach (var extension in associatedExtensions)
        {
            var filepath = Path.ChangeExtension(_databaseFilePath, extension);
            if (File.Exists(filepath)) File.Delete(filepath);
        }

        return ValueTask.CompletedTask;
    }

    public DbConnection GetConnection() => new SqliteConnection(_connectionString ?? throw NewDatabaseNotInitializedException());

    public string GetConnectionString() => _connectionString ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new("Database has not been initialized");
}
