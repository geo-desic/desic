using Microsoft.Data.SqlClient;

namespace Desic.Testing.Integration.Db;

public class EmptyDatabaseSqlServerBasedOnConfig(bool contained = true) : IDatabaseSqlServer
{
    private readonly bool _contained = contained;
    private IDatabaseSqlServer? _database;
    private readonly IntegrationTestsOptions? _options = TestConfiguration.Options;

    public SqlConnection GetSqlServerConnection() => _database?.GetSqlServerConnection() ?? throw Exceptions.DatabaseNotInitialized();
    public string GetConnectionString() => _database?.GetConnectionString() ?? throw Exceptions.DatabaseNotInitialized();
    public string DatabaseName => _database?.DatabaseName ?? throw Exceptions.DatabaseNotInitialized();

    public async ValueTask InitializeAsync()
    {
        if (_options?.DbProviders?.SqlServer?.UseContainer ?? false) // container
        {
            var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? Containers.DefaultImageSqlServer;
            Console.WriteLine($"Using database: {DbProvider.SqlServer} (container) {image}");
            _database = new EmptyDatabaseSqlServerContainer(contained: _contained, image: image);
        }
        else // local
        {
            var connectionString = _options?.ConnectionStrings?.SqlServer ?? throw new InvalidOperationException($"Connection string for local {DbProvider.SqlServer} could not be determined");
            Console.WriteLine($"Using database: {DbProvider.SqlServer} (local)");
            _database = new EmptyDatabaseSqlServerLocal(contained: _contained, connectionString: connectionString);
        }
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_database != null) await _database.DisposeAsync();
    }
}
