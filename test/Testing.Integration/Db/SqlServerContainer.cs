using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SqlServerContainer(string image = Containers.DefaultImageSqlServer, IWaitForContainerOS? waitStrategy = null) : ISqlServer
{
    private string? _connectionString;
    private readonly MsSqlContainer _container = waitStrategy == null ? new MsSqlBuilder(image).Build() : new MsSqlBuilder(image).WithWaitStrategy(waitStrategy).Build();
    private readonly string _image = image ?? throw new ArgumentNullException(nameof(image));

    public SqlConnection GetSqlServerConnection() => new(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        Console.WriteLine($"Successfully started SqlServer container created from image: {_image}");

        _connectionString = _container.GetConnectionString(); // if using standard microsoft images this is a master (sa) database connection as no user databases exist yet

        using var connection = GetSqlServerConnection();
        await connection.OpenAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
