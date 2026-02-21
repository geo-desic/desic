using Desic.Api.Db;
using Desic.Data;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

// class is sealed for simpler IAsyncLifetime implementation
public sealed class DesicContextLocalDb(string apiUserPassword) : IAsyncLifetime
{
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");
    private string? _connectionStringApp;
    private string? _connectionStringMigrations;
    private string? _databaseFilePath;
    private string? _databaseFileName;
    private string? _databaseName;
    private const string DataSource = @"(localdb)\MSSQLLocalDB";

    public string ConnectionString => _connectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionString)} has not been initialized");

    public async ValueTask InitializeAsync()
    {
        // make sure temporary directory for the database files exists
        var tempDir = Path.Combine(Path.GetTempPath(), "desic-tests");
        Directory.CreateDirectory(tempDir);

        // create a unique name for the database
        _databaseName = $"desic_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _databaseFileName = $"{_databaseName}.mdf";
        _databaseFilePath = Path.Combine(tempDir, _databaseFileName);

        _connectionStringMigrations = $"Data Source={DataSource};Initial Catalog={_databaseName};Integrated Security=True;";

        // create the database and apply migrations
        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", _connectionStringMigrations]);

        await context.InitializeAsync(targetDatabaseName: _databaseName);
        await context.Database.MigrateAsync();

        _connectionStringApp = $"Data Source={DataSource};Initial Catalog={_databaseName};User ID={Providers.DbApiUser};Password={_apiUserPassword};";

        using var connection = new SqlConnection(_connectionStringApp);
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseFilePath))
        {
            using var connection = new SqlConnection($"Data Source={DataSource};Integrated Security=True;");
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"IF DB_ID('{_databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]; END";
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();

            try { File.Delete(_databaseFilePath); } catch { /* nothing */ }
            try { File.Delete(Path.ChangeExtension(_databaseFilePath, ".ldf")); } catch { /* nothing */ }
        }
    }
}
