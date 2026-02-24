using Desic.Api.Db;
using Desic.Data;
using Desic.Infrastructure.Data.SqlServer;
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
    private string? _connectionString;
    private readonly string _apiUserPassword = apiUserPassword ?? throw new InvalidOperationException("Api user password could not be determined");

    public string ConnectionString => _connectionString ?? throw new InvalidOperationException($"{nameof(ConnectionString)} has not been initialized");

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var connectionStringInit = new SqlConnectionStringBuilder(_container.GetConnectionString()) { InitialCatalog = "Desic" }.ConnectionString;

        // create the database and apply migrations
        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", connectionStringInit]);

        await context.InitializeAsync(targetDatabaseName: "Desic");
        await context.Database.MigrateAsync();

        var builder = new SqlConnectionStringBuilder(connectionStringInit)
        {
            UserID = Providers.DbApiUser,
            Password = _apiUserPassword,
        };
        _connectionString = builder.ConnectionString;

        using var connection = new SqlConnection(_connectionString);
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
