using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseBasedOnConfig : ITestDatabase
{
    private ITestDatabase? _database;
    private readonly IntegrationTestsOptions? _options = TestConfiguration.Options;

    public async ValueTask InitializeAsync()
    {
        if (DbProvider == DbProvider.Sqlite)
        {
            Console.WriteLine($"Using database: {DbProvider}");
            _database = new TestDatabaseSqlite();
        }
        else // sql server
        {
            var apiUserPassword = _options?.Databases?.Application?.SqlServer?.Users?["Api"]?.Password ?? throw new InvalidOperationException($"Api user {nameof(IntegrationTestsDatabaseApplicationSqlServerUsersOptions.Password)} is not configured");
            if (_options?.DbProviders?.SqlServer?.UseContainer ?? false)
            {
                var image = _options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException($"Container image for sql server is not configured");
                Console.WriteLine($"Using database: {DbProvider} (container)");
                _database = new TestDatabaseMsSqlContainer(image: image, apiUserPassword: apiUserPassword);
            }
            else // localdb
            {
                Console.WriteLine($"Using database: {DbProvider} (localdb)");
                _database = new TestDatabaseLocalDb(apiUserPassword: apiUserPassword);
            }
        }
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _database?.DisposeAsync().AsTask().Wait();
    }

    public DbProvider DbProvider => _options?.DbProvider == DbProvider.Sqlite ? DbProvider.Sqlite : DbProvider.SqlServer;

    public DbConnection GetConnection() => _database?.GetConnection() ?? throw NewDatabaseNotInitializedException();

    public string GetConnectionString() => _database?.GetConnectionString() ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new("Database has not been initialized");
}
