using Desic.Api.Db;
using Desic.Data;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TemplateDatabaseLocalDb(string databaseDirectoryPath, string apiUserPassword) : ITemplateDatabase
{
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");
    private string? _connectionStringApi;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private string? _databaseBackupFilePath;
    private string? _databaseName;
    public const string DataSource = @"(localdb)\MSSQLLocalDB";

    public ITestDatabase NewTestDatabase()
    {
        if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(_databaseBackupFilePath)) throw new InvalidOperationException(Constants.DatabaseNotInitialized);
        return new TestDatabaseLocalDb(databaseDirectoryPath: _databaseDirectoryPath, templateDatabaseName: _databaseName, templateDatabaseBackupFilePath: _databaseBackupFilePath, apiUserPassword: _apiUserPassword);
    }

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_template_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes

         var connectionStringMigrations = $"Data Source={DataSource};Initial Catalog={_databaseName};Integrated Security=True;";

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionStringMigrations, "--environment", Constants.TestEnvironmentName]);

        await context.InitializeAsync(targetDatabaseName: _databaseName);
        Console.Write($"Successfully initialized database: {_databaseName}");
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {_databaseName}");

        _connectionStringApi = $"Data Source={DataSource};Initial Catalog={_databaseName};User ID={Providers.DbApiUser};Password={_apiUserPassword};";

        using var connection = new SqlConnection(_connectionStringApi);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
        Console.Write($"Successfully connected to database {_databaseName} as user: {Providers.DbApiUser}");

        await BackupDatabase();
        Console.Write($"Successfully backed up database: {_databaseBackupFilePath}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseBackupFilePath))
        {
            try { File.Delete(_databaseBackupFilePath); } catch { /* nothing */ }
        }
        using var connection = new SqlConnection($"Data Source={DataSource};Integrated Security=True;");
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"IF DB_ID('{_databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]; END";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    private async Task BackupDatabase()
    {
        var databaseBackupFilePath = Path.Combine(_databaseDirectoryPath, $"{_databaseName}.bak");
        using var connection = new SqlConnection($"Data Source={DataSource};Integrated Security=True;");
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"BACKUP DATABASE [{_databaseName}] TO DISK = N'{databaseBackupFilePath}' WITH FORMAT, NAME = N'Full Backup of [{_databaseName}]';";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
        _databaseBackupFilePath = databaseBackupFilePath;
    }
}
