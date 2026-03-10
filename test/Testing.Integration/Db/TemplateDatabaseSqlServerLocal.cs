using Desic.Api.Db;
using Desic.Data;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TemplateDatabaseSqlServerLocal(string connectionStringInitialization, string databaseDirectoryPath, DatabaseInitializerOptions databaseInitializerOptions) : ITemplateDatabase
{
    private readonly string _apiUserPassword = databaseInitializerOptions.Users?.Api?.Password ?? throw new InvalidOperationException($"{nameof(DatabaseInitializerUsersOptions.Api)} user {nameof(DatabaseInitializerUserOptions.Password)} is not configured");
    private string? _connectionStringApi;
    private string? _connectionStringMigrations;
    private readonly string _connectionStringInitialization = connectionStringInitialization ?? throw new ArgumentNullException(nameof(connectionStringInitialization));
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private string? _databaseBackupFilePath;
    private readonly DatabaseInitializerOptions _databaseInitializerOptions = databaseInitializerOptions ?? throw new ArgumentException(nameof(databaseInitializerOptions));
    private string? _databaseName;

    public string BackupFilePath => _databaseBackupFilePath ?? throw NewInvalidOperationException();
    public string ConnectionStringApi => _connectionStringApi ?? throw NewInvalidOperationException();
    public string ConnectionStringInitialization => _connectionStringInitialization;
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw NewInvalidOperationException();
    public string DirectoryPath => _databaseDirectoryPath;
    public string Name => _databaseName ?? throw NewInvalidOperationException();
    public DatabaseInitializerUsersOptions UsersOptions => _databaseInitializerOptions.Users ?? throw NewInvalidOperationException();

    public ITestDatabase NewTestDatabase()
    {
        if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(_databaseBackupFilePath)) throw new InvalidOperationException(Constants.DatabaseNotInitialized);
        return new TestDatabaseSqlServerLocal(templateDatabase: this);
    }

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_template_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes
        _connectionStringMigrations = new SqlConnectionStringBuilder(_connectionStringInitialization) { InitialCatalog = _databaseName }.ConnectionString;
        _connectionStringApi = new SqlConnectionStringBuilder(_connectionStringMigrations) { UserID = Providers.DbApiUser, Password = _apiUserPassword }.ConnectionString;

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", _connectionStringMigrations, "--environment", Constants.TestEnvironmentName]);

        await context.InitializeAsync(targetDatabaseName: _databaseName);
        Console.Write($"Successfully initialized database: {_databaseName}");
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {_databaseName}");

        using var connection = new SqlConnection(_connectionStringApi);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
        Console.Write($"Successfully connected to database {_databaseName} as user: {Providers.DbApiUser}");

        await BackupDatabase();
        Console.Write($"Successfully backed up database: {_databaseBackupFilePath}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseBackupFilePath)) try { File.Delete(_databaseBackupFilePath); } catch { /* nothing */ }
        if (!string.IsNullOrEmpty(_databaseName)) await DropDatabase(connectionString: _connectionStringInitialization, databaseName: _databaseName);
    }

    internal static async Task DropDatabase(string connectionString, string databaseName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"IF DB_ID('{databaseName}') IS NOT NULL BEGIN ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]; END";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
    }

    private async Task BackupDatabase()
    {
        var databaseBackupFilePath = Path.Combine(_databaseDirectoryPath, $"{_databaseName}.bak");
        using var connection = new SqlConnection(_connectionStringInitialization);
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"BACKUP DATABASE [{_databaseName}] TO DISK = N'{databaseBackupFilePath}' WITH FORMAT, NAME = N'Full Backup of [{_databaseName}]';";
            await command.ExecuteNonQueryAsync();
        }
        await connection.CloseAsync();
        _databaseBackupFilePath = databaseBackupFilePath;
    }

    private static InvalidOperationException NewInvalidOperationException() => new(Constants.DatabaseNotInitialized);
}
