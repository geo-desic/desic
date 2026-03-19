using Desic.Shared.Data;
using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Testcontainers.MsSql;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class SeededAppDatabaseSqlServerContainer(SeededAppTemplateDatabaseSqlServerContainer templateDatabase) : ITestDatabase
{
    private string? _connectionString;
    // waiting for both log messages seems to be important to ensure not only that the server is online and accepting requests, but the desired database is ready
    // however relying on specific log messages is not ideal as they are subject to change
    private readonly MsSqlContainer _container = new MsSqlBuilder(templateDatabase.TemplateImage)
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilMessageIsLogged(ServerReadyForConnectionsMessage, o => o.WithTimeout(DatabaseReadyTimeout))
            .UntilMessageIsLogged(DatabaseStartedLogMessage, o => o.WithTimeout(DatabaseReadyTimeout))).Build();
    private static readonly TimeSpan DatabaseReadyTimeout = TimeSpan.FromSeconds(10);
    private const string DatabaseStartedLogMessage = $"Starting up database '{Constants.DatabaseName}'";
    private const string ServerReadyForConnectionsMessage = "SQL Server is now ready for client connections";
    private readonly SeededAppTemplateDatabaseSqlServerContainer _templateDatabase = templateDatabase ?? throw new ArgumentNullException(nameof(templateDatabase));

    public DbConnection GetConnection() => new SqlConnection(_connectionString ?? throw Exceptions.DatabaseNotInitialized());
    public string GetConnectionString() => _connectionString ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var dataSource = new SqlConnectionStringBuilder(_container.GetConnectionString()).DataSource;
        _connectionString = new SqlConnectionStringBuilder(_templateDatabase.ConnectionStringApi) { DataSource = dataSource }.ConnectionString;

        using var connection = GetConnection();
        if (!await connection.TryOpenAsync()) throw new Exception("Unable to connect to the database using the api connection string");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
