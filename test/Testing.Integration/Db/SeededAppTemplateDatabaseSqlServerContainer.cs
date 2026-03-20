using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Providers;
using Desic.Infrastructure.Data.SqlServer;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppTemplateDatabaseSqlServerContainer(string image) : ITemplateDatabase
{
    private string? _connectionStringApi;
    private string? _connectionStringInitialization;
    private string? _connectionStringMigrations;
    private readonly MsSqlContainer _container = new MsSqlBuilder(image).Build();
    private readonly string _databaaseName = Constants.DatabaseName;
    private const string TemplateImageRepositoryNamePrefix = "mssql-int-tests";
    private const string TemplateImageTag = "temporary";
    private string? _templateImage;

    public string ConnectionStringApi => _connectionStringApi ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringInitialization => _connectionStringInitialization ?? throw Exceptions.DatabaseNotInitialized();
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseName => _databaaseName;
    public string TemplateImage => _templateImage ?? throw Exceptions.DatabaseNotInitialized();

    public ITestDatabase NewTestDatabase() => new SeededAppDatabaseSqlServerContainer(this);

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Console.Write($"Successfully started SqlServer container: {image}");

        _connectionStringInitialization = _container.GetConnectionString();

        var connectionStringDatabase = new SqlConnectionStringBuilder(_connectionStringInitialization) { InitialCatalog = DatabaseName, UserID = string.Empty, Password = string.Empty }.ConnectionString;

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
            DatabaseName = _databaaseName,
        };
        await mediator.Send(request: request, cancellationToken: default);
        Console.Write($"Successfully initialized database: {_databaaseName}");

        // apply migrations
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {_databaaseName}");

        // ensure can connect to the database as the api user
        var hostConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _connectionStringApi = hostConfiguration.GetSqlServerConnectionString(ConnectionStringType.Api);
        using var connection = new SqlConnection(_connectionStringApi);
        await connection.OpenAsync();
        await connection.CloseAsync();
        Console.Write($"Successfully connected to database as api user");

        var sqlCmdFilePath = await _container.GetSqlCmdFilePathAsync().ConfigureAwait(false);
        await _container.ExecAsync([sqlCmdFilePath, "-C", "-Q", "SHUTDOWN"]).ConfigureAwait(false);
        Console.Write("Successfully cleanly shutdown database server");

        await _container.StopAsync();
        Console.Write("Successfully stopped SqlServer container");

        using var clientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);
        using var client = clientConfiguration.CreateClient();
        var model = new Docker.DotNet.Models.CommitContainerChangesParameters
        {
            ContainerID = _container.Id,
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
}
