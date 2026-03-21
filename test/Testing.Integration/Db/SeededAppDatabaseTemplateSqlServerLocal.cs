using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Providers;
using Desic.Infrastructure.Data.SqlServer;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseTemplateSqlServerLocal(string connectionStringInitialization, string backupDirectoryPath, InitializeApplicationDatabaseOptions options) : IDatabaseTemplate, IDatabaseSqlServer
{
    private List<SqlServerDatabaseFile>? _backupFileList;
    private string? _connectionStringApi;
    private readonly string _connectionStringInitialization = connectionStringInitialization ?? throw new ArgumentNullException(nameof(connectionStringInitialization));
    private string? _connectionStringMigrations;
    private bool? _contained;
    private readonly string _backupDirectoryPath = backupDirectoryPath ?? throw new ArgumentNullException(nameof(backupDirectoryPath));
    private string? _databaseBackupFilePath;
    private readonly InitializeApplicationDatabaseOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly string _databaseName = $"{Constants.DatabaseName.ToLowerInvariant()}_template_{Guid.CreateVersion7():N}";

    public IReadOnlyList<SqlServerDatabaseFile> GetBackupFileList() => _backupFileList?.AsReadOnly() ?? throw Exceptions.DatabaseNotInitialized();
    public string BackupFilePath => _databaseBackupFilePath ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringApi => _connectionStringApi ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringInitialization => _connectionStringInitialization ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseName => _databaseName;
    public SqlConnection GetSqlServerConnection() => new(_connectionStringInitialization ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionStringInitialization ?? throw Exceptions.DatabaseNotInitialized();
    public bool IsContained => _contained ?? throw Exceptions.DatabaseNotInitialized();
    public InitializeApplicationDatabaseUsersOptions UsersOptions => _options.Users ?? throw Exceptions.DatabaseNotInitialized();

    public IDatabase NewDatabase()
    {
        if (string.IsNullOrEmpty(_databaseBackupFilePath)) throw Exceptions.DatabaseNotInitialized();
        return new SeededAppDatabaseSqlServerLocal(databaseTemplate: this);
    }

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_backupDirectoryPath);

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

        _contained = await SqlServerOperations.IsContained(_connectionStringInitialization, _databaseName);

        // apply migrations
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {_databaseName}");

        // ensure can connect to the database as the api user
        var hostConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _connectionStringApi = hostConfiguration.GetSqlServerConnectionString(ConnectionStringType.Api);
        using var connection = new SqlConnection(_connectionStringApi);
        await connection.OpenAsync();
        await connection.CloseAsync();
        Console.Write($"Successfully connected to database [{_databaseName}] as api user");

        var databaseBackupFilePath = Path.Combine(_backupDirectoryPath, $"{_databaseName}.bak");
        _backupFileList = await SqlServerOperations.BackupDatabaseReturningFileList(connectionString: _connectionStringInitialization, databaseBackupFilePath: databaseBackupFilePath, databaseName: _databaseName);
        _databaseBackupFilePath = databaseBackupFilePath;
        Console.Write($"Successfully backed up database: {_databaseBackupFilePath}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_databaseBackupFilePath)) try { File.Delete(_databaseBackupFilePath); } catch { /* nothing */ }
        await SqlServerOperations.DropDatabase(connectionString: _connectionStringInitialization, databaseName: _databaseName);
    }
}
