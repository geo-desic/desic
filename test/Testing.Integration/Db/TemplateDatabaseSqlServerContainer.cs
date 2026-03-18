using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Providers;
using Desic.Infrastructure.Data.SqlServer;
using Desic.Shared.Data;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TemplateDatabaseSqlServerContainer(string image) : ITemplateDatabase
{
    private string? _connectionStringApi;
    private string? _connectionStringInitialization;
    private string? _connectionStringMigrations;
    private readonly MsSqlContainer _container = new MsSqlBuilder(image).Build();
    private const string TemplateImageRepositoryNamePrefix = "mssql-int-tests";
    private const string TemplateImageTag = "temporary";
    private string? _templateImage;

    public string ConnectionStringApi => _connectionStringApi ?? throw NewInvalidOperationException();
    public string ConnectionStringInitialization => _connectionStringInitialization ?? throw NewInvalidOperationException();
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw NewInvalidOperationException();
    public string TemplateImage => _templateImage ?? throw NewInvalidOperationException();

    public ITestDatabase NewTestDatabase() => new TestDatabaseSqlServerContainer(this);

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Console.Write($"Successfully started SqlServer container: {image}");

        _connectionStringInitialization = _container.GetConnectionString();

        var connectionStringDatabase = new SqlConnectionStringBuilder(_connectionStringInitialization) { InitialCatalog = Constants.DatabaseName, UserID = string.Empty, Password = string.Empty }.ConnectionString;

        var hostBuilder = ApplicationDbContextFactory.CreateHostBuilder(["--ConnectionStrings:SqlServer", connectionStringDatabase, "--environment", Constants.TestEnvironmentName]);
        _connectionStringMigrations = hostBuilder.Configuration.GetSqlServerConnectionString(ConnectionStringType.Migrations);
        hostBuilder.Services.AddSqlServerInfrastructure(hostBuilder.Configuration, _connectionStringMigrations); // re-add with migrations connection string
        using var host = hostBuilder.Build();
        using var scope = host.Services.CreateScope();

        // create/initialize the database
        var databaseInitializer = scope.ServiceProvider.GetRequiredService<InitializeApplicationDatabaseRequest>();
        await databaseInitializer.InitializeAsync(connectionString: _connectionStringInitialization, targetDatabaseName: Constants.DatabaseName);
        Console.Write($"Successfully initialized database: {Constants.DatabaseName}");

        // apply migrations
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {Constants.DatabaseName}");

        // ensure can connect to the database as the api user
        var hostConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _connectionStringApi = hostConfiguration.GetSqlServerConnectionString(ConnectionStringType.Api);
        using var connection = new SqlConnection(_connectionStringApi);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
        await connection.CloseAsync();
        Console.Write($"Successfully connected to database as api user");

        var sqlCmdFilePath = await _container.GetSqlCmdFilePathAsync().ConfigureAwait(false);
        await _container.ExecAsync([sqlCmdFilePath, "-C", "-Q", "SHUTDOWN"]).ConfigureAwait(false);
        Console.Write("Successfully shutdown database server");

        await _container.StopAsync();
        Console.Write("Successfully stopped SqlServer container");

        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);
        using var client = clientConfiguration.CreateClient();

        var containerId = _container.Id;
        var model = new Docker.DotNet.Models.CommitContainerChangesParameters
        {
            ContainerID = containerId,
            RepositoryName = $"{TemplateImageRepositoryNamePrefix}-{Guid.NewGuid()}",
            Tag = TemplateImageTag,
        };
        await client.Images.CommitContainerChangesAsync(model);
        _templateImage = $"{model.RepositoryName}:{model.Tag}";
        Console.Write($"Successfully created a temporary docker image of the SqlServer container: {_templateImage}");
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private static InvalidOperationException NewInvalidOperationException() => new(Constants.DatabaseNotInitialized);
}
