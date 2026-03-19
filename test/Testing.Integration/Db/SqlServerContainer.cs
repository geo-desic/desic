using Desic.Shared.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SqlServerContainer(string image = Containers.DefaultImageSqlServer) : ITestDatabase
{
    private string? _connectionString;
    private readonly MsSqlContainer _container = new MsSqlBuilder(image).Build();

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());

    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Console.Write($"Successfully started SqlServer container: {image}");

        _connectionString = _container.GetConnectionString(); // if using standard microsoft images this is a master (sa) database connection as no user databases exist yet

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the container database");
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
