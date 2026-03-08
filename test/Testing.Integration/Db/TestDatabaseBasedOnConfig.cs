using System.Data.Common;

namespace Desic.Testing.Integration.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class TestDatabaseBasedOnConfig : ITestDatabase
{
    private ITestDatabase? _database;
    private readonly bool _useContainerSqlServer = TestConfiguration.Options?.DbProviders?.SqlServer?.UseContainer ?? false;

    public async ValueTask InitializeAsync()
    {
        if (TestConfiguration.Options?.DbProvider == "Sqlite")
        {
            Console.WriteLine($"Using database: {TestConfiguration.Options.DbProvider}");
            _database = new TestDatabaseSqlite();
        }
        else // sql server
        {
            var apiUserPassword = TestConfiguration.Options?.Databases?.Application?.SqlServer?.Users?["Api"]?.Password ?? throw new InvalidOperationException($"Api user {nameof(IntegrationTestsDatabaseApplicationSqlServerUsersOptions.Password)} is not configured");
            if (_useContainerSqlServer)
            {
                var image = TestConfiguration.Options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException($"Container image for sql server is not configured");
                Console.WriteLine($"Using database: {TestConfiguration.Options.DbProvider} (container)");
                _database = new TestDatabaseMsSqlContainer(image: image, apiUserPassword: apiUserPassword);
            }
            else // localdb
            {
                Console.WriteLine($"Using database: {TestConfiguration.Options.DbProvider} (localdb)");
                _database = new TestDatabaseLocalDb(apiUserPassword: apiUserPassword);
            }
        }
        await _database.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _database?.DisposeAsync().AsTask().Wait();
    }

    public DbConnection GetConnection() => _database?.GetConnection() ?? throw NewDatabaseNotInitializedException();

    public string GetConnectionString() => _database?.GetConnectionString() ?? throw NewDatabaseNotInitializedException();

    private static InvalidOperationException NewDatabaseNotInitializedException() => new("Database has not been initialized");
}
