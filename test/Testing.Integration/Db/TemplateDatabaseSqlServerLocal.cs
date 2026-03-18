using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Providers;
using Desic.Infrastructure.Data.SqlServer;
using Desic.Shared.Data;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TemplateDatabaseSqlServerLocal(string connectionStringInitialization, string databaseDirectoryPath, InitializeApplicationDatabaseOptions databaseInitializerOptions) : ITemplateDatabase
{
    private string? _connectionStringApi;
    private readonly string _connectionStringInitialization = connectionStringInitialization ?? throw new ArgumentNullException(nameof(connectionStringInitialization));
    private string? _connectionStringMigrations;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private string? _databaseBackupFilePath;
    private readonly InitializeApplicationDatabaseOptions _databaseInitializerOptions = databaseInitializerOptions ?? throw new ArgumentException(nameof(databaseInitializerOptions));
    private string? _databaseName;

    public string BackupFilePath => _databaseBackupFilePath ?? throw NewInvalidOperationException();
    public string ConnectionStringApi => _connectionStringApi ?? throw NewInvalidOperationException();
    public string ConnectionStringInitialization => _connectionStringInitialization ?? throw NewInvalidOperationException();
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw NewInvalidOperationException();
    public string DirectoryPath => _databaseDirectoryPath;
    public string Name => _databaseName ?? throw NewInvalidOperationException();
    public InitializeApplicationDatabaseUsersOptions UsersOptions => _databaseInitializerOptions.Users ?? throw NewInvalidOperationException();

    public ITestDatabase NewTestDatabase()
    {
        if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(_databaseBackupFilePath)) throw new InvalidOperationException(Constants.DatabaseNotInitialized);
        return new TestDatabaseSqlServerLocal(templateDatabase: this);
    }

    public async ValueTask InitializeAsync()
    {
        // create a unique name for the database
        _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_template_{Guid.CreateVersion7():N}"; // uuidv7 will be easier to sort in file explorer for debugging purposes

        var connectionStringDatabase = new SqlConnectionStringBuilder(_connectionStringInitialization) { InitialCatalog = _databaseName, IntegratedSecurity = false }.ConnectionString;

        var hostBuilder = ApplicationDbContextFactory.CreateHostBuilder(["--ConnectionStrings:SqlServer", connectionStringDatabase, "--environment", Constants.TestEnvironmentName]);
        _connectionStringMigrations = hostBuilder.Configuration.GetSqlServerConnectionString(ConnectionStringType.Migrations);
        hostBuilder.Services.AddSqlServerInfrastructure(hostBuilder.Configuration, _connectionStringMigrations); // re-add with migrations connection string
        using var host = hostBuilder.Build();
        using var scope = host.Services.CreateScope();

        // create/initialize the database
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var request = new InitializeApplicationDatabaseRequest
        {
            ConnectionString = _connectionStringInitialization,
            DatabaseName = _databaseName,
        };
        await mediator.Send(request: request, cancellationToken: default);
        Console.Write($"Successfully initialized database: {_databaseName}");

        // apply migrations
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {_databaseName}");

        // ensure can connect to the database as the api user
        var hostConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _connectionStringApi = hostConfiguration.GetSqlServerConnectionString(ConnectionStringType.Api);
        using var connection = new SqlConnection(_connectionStringApi);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
        await connection.CloseAsync();
        Console.Write($"Successfully connected to database {_databaseName} as api user");

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
