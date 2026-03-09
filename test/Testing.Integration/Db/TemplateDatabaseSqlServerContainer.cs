using Desic.Api.Db;
using Desic.Data;
using Desic.Infrastructure.Data.SqlServer;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TemplateDatabaseSqlServerContainer(string image, string apiUserPassword) : ITemplateDatabase
{
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");
    private string? _connectionString;
    private readonly MsSqlContainer _container = new MsSqlBuilder(image).Build();
    private const string TemplateImageRepositoryNamePrefix = "mssql-int-tests";
    private const string TemplateImageTag = "temporary";
    private string? _templateImage;

    public ITestDatabase NewTestDatabase() => new TestDatabaseSqlServerContainer(_templateImage ?? throw new InvalidOperationException(Constants.DatabaseNotInitialized), _apiUserPassword);

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Console.Write($"Successfully started SqlServer container: {image}");

        var connectionStringInit = new SqlConnectionStringBuilder(_container.GetConnectionString()) { InitialCatalog = Constants.DatabaseName }.ConnectionString;

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionStringInit, "--environment", Constants.TestEnvironmentName]);

        await context.InitializeAsync(targetDatabaseName: Constants.DatabaseName);
        Console.Write($"Successfully initialized database: {Constants.DatabaseName}");
        await context.Database.MigrateAsync();
        Console.Write($"Successfully migrated database: {Constants.DatabaseName}");

        var builder = new SqlConnectionStringBuilder(connectionStringInit)
        {
            UserID = Providers.DbApiUser,
            Password = _apiUserPassword,
        };
        _connectionString = builder.ConnectionString;

        using var connection = new SqlConnection(_connectionString);
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
        Console.Write($"Successfully connected to database {builder.InitialCatalog} as user: {builder.UserID}");
        await connection.CloseAsync();

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
}
