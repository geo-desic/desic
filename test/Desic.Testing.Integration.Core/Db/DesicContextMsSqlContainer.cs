using Desic.Api.Db;
using Desic.Core.Data;
using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class DesicContextMsSqlContainer(string image, string apiUserPassword) : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(image ?? throw new InvalidOperationException("Container image could not be determined")).Build();
    private string? _connectionStringApp;
    private string? _connectionStringMigrations;
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");

    public string ConnectionStringApp => _connectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionStringApp)} has not been initialized");
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw new InvalidOperationException($"{nameof(ConnectionStringMigrations)} has not been initialized");

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        _connectionStringMigrations = new SqlConnectionStringBuilder(_container.GetConnectionString()) { InitialCatalog = "Desic" }.ConnectionString;

        // create the database and apply migrations
        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", ConnectionStringMigrations]);

        await context.InitializeAsync(targetDatabaseName: "Desic");
        await context.Database.MigrateAsync();

        var builder = new SqlConnectionStringBuilder(ConnectionStringMigrations)
        {
            UserID = Providers.DbApiUser,
            Password = _apiUserPassword,
        };
        _connectionStringApp = builder.ConnectionString;

        using var connection = new SqlConnection(_connectionStringApp);
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
