using Desic.Infrastructure.Data.Sqlite;
using Desic.Shared.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppTemplateDatabaseSqlite(string databaseDirectoryPath) : ITemplateDatabase
{
    private string? _databaseFilePath;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));

    public ITestDatabase NewTestDatabase() => new SeededAppDatabaseSqlite(_databaseFilePath ?? throw Exceptions.DatabaseNotInitialized());

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        var databaseFileName = $"{Constants.DatabaseName.ToLowerInvariant()}_template_{Guid.CreateVersion7():N}.db"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFilePath = Path.Combine(_databaseDirectoryPath, databaseFileName);

        var connectionString = $"Data Source={_databaseFilePath};Pooling=False;"; // pooling is disabled to avoid issues with file locks when deleting the database file after tests are done

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionString, "--environment", Constants.TestEnvironmentName]);

        await context.Database.MigrateAsync();

        using var connection = new SqliteConnection(connectionString);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database");
        await connection.CloseAsync();
    }

    public ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath)) SqliteOperations.DeleteDatabaseAndAssociatedFiles(_databaseFilePath);
        return ValueTask.CompletedTask;
    }
}
