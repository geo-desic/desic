using Desic.Infrastructure.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

public sealed class DesicContextSqlite : IAsyncLifetime
{
    private string? _connectionString;
    private string? _databaseFilePath;

    public string ConnectionString => _connectionString ?? throw new InvalidOperationException($"{nameof(ConnectionString)} has not been initialized");

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
        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", ConnectionString]);

        await context.Database.MigrateAsync();
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
}
