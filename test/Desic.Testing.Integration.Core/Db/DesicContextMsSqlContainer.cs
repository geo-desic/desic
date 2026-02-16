using Desic.EntityFrameworkCore.DbConnections;
using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class DesicContextMsSqlContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(TestSettingsConfiguration.Root.GetValue<string>("Containers:MsSql:Image")).Build();
    private string? _connectionStringApp;
    private string? _connectionStringMigrations;

    public string ConnectionStringApp => _connectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionStringApp)} has not been initialized");
    public string ConnectionStringMigrations => _connectionStringMigrations ?? throw new InvalidOperationException($"{nameof(ConnectionStringMigrations)} has not been initialized");

    public async ValueTask InitializeAsync()
    {
        Console.WriteLine($"Starting mssql container");
        await _container.StartAsync();
        Console.WriteLine($"Started mssql container with name = {_container.Name} and id = {_container.Id} and image = {_container.Image.FullName}");

        _connectionStringMigrations = _container.GetConnectionString();
        Console.WriteLine($"Attempting to instantiate a DesicContext");
        using var factory = new DesicContextFactory();
        using var context = factory.CreateDbContext(["--connection", ConnectionStringMigrations]);
        Console.WriteLine($"Successfully instantiated a DesicContext");

        Console.WriteLine($"Attempting to initialize the database");
        using var cts = new CancellationTokenSource();
        await context.Database.MigrateAsync(cts.Token);
        Console.WriteLine($"Successfully initialized the database");

        var configKey = "Databases:Desic:AppUserPassword";
        var appUserPassword = TestSettingsConfiguration.Root.GetValue<string>(configKey);
        if (string.IsNullOrEmpty(appUserPassword)) throw new Exception($"Configuration value not set: {configKey}");
        var builder = new SqlConnectionStringBuilder(ConnectionStringMigrations)
        {
            UserID = DesicContext.AppUser,
            Password = appUserPassword,
        };
        _connectionStringApp = builder.ConnectionString;

        using var connection = new SqlConnection(_connectionStringApp);
        if (!await connection.CanConnectAsync()) throw new Exception($"Failed to connect to the database using the app connection string");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
