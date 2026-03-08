using Desic.Api.Db;
using Desic.Data;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseMsSqlContainer(string image, string apiUserPassword) : ITestDatabase
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(image ?? throw new InvalidOperationException("Container image could not be determined")).Build();
    private string? _connectionString;
    private const string DatabaseName = "Desic";
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var connectionStringInit = new SqlConnectionStringBuilder(_container.GetConnectionString()) { InitialCatalog = DatabaseName }.ConnectionString;

        // create the database and apply migrations
        using var factory = new ApplicationDbContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionStringInit, "--environment", Constants.TestEnvironmentName]);

        await context.InitializeAsync(targetDatabaseName: DatabaseName);
        await context.Database.MigrateAsync();

        var builder = new SqlConnectionStringBuilder(connectionStringInit)
        {
            UserID = Providers.DbApiUser,
            Password = _apiUserPassword,
        };
        _connectionString = builder.ConnectionString;

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw NewDatabaseNotInitializedException());

    public string GetConnectionString() => _connectionString ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new("Database has not been initialized");
}
