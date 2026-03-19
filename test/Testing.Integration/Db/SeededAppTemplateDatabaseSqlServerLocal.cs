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
public sealed class SeededAppTemplateDatabaseSqlServerLocal(string connectionStringInitialization, string databaseDirectoryPath, InitializeApplicationDatabaseOptions options) : ITemplateDatabase
{
    private string? _connectionStringApi;
    private readonly string _connectionStringInitialization = connectionStringInitialization ?? throw new ArgumentNullException(nameof(connectionStringInitialization));
    private string? _connectionStringMigrations;
    private readonly string _databaseDirectoryPath = databaseDirectoryPath ?? throw new ArgumentNullException(nameof(databaseDirectoryPath));
    private string? _databaseBackupFilePath;
    private readonly InitializeApplicationDatabaseOptions _options = options ?? throw new ArgumentException(nameof(options));
    private string? _databaseName;

    public string BackupFilePath => _databaseBackupFilePath ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringApi => _connectionStringApi ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringInitialization => _connectionStringInitialization ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw Exceptions.DatabaseNotInitialized();
    public string DirectoryPath => _databaseDirectoryPath;
    public string Name => _databaseName ?? throw Exceptions.DatabaseNotInitialized();
    public InitializeApplicationDatabaseUsersOptions UsersOptions => _options.Users ?? throw Exceptions.DatabaseNotInitialized();

    public ITestDatabase NewTestDatabase()
    {
        if (string.IsNullOrEmpty(_databaseName) || string.IsNullOrEmpty(_databaseBackupFilePath)) throw new InvalidOperationException(Constants.DatabaseNotInitialized);
        return new SeededAppDatabaseSqlServerLocal(templateDatabase: this);
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
        Console.Write($"Successfully connected to database [{_databaseName}] as api user");

        var databaseBackupFilePath = Path.Combine(_databaseDirectoryPath, $"{_databaseName}.bak");
        await SqlServerOperations.BackupDatabase(connectionString: _connectionStringInitialization, databaseBackupFilePath: databaseBackupFilePath, databaseName: _databaseName);
        _databaseBackupFilePath = databaseBackupFilePath;
        Console.Write($"Successfully backed up database: {_databaseBackupFilePath}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseBackupFilePath)) try { File.Delete(_databaseBackupFilePath); } catch { /* nothing */ }
        if (!string.IsNullOrEmpty(_databaseName)) await SqlServerOperations.DropDatabase(connectionString: _connectionStringInitialization, databaseName: _databaseName);
    }
}
