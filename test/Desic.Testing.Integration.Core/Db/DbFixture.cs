using Xunit;

namespace Desic.Testing.Integration.Core.Db;

public sealed class DbFixture : IAsyncLifetime
{
    private DesicContextMsSqlContainer? _container;
    private DesicContextLocalDb? _localDb;
    private DesicContextSqlite? _sqlite;
    private readonly bool _useContainerSqlServer = TestConfiguration.Options?.DbProviders?.SqlServer?.UseContainer ?? false;

    public string ConnectionStringApp
    {
        get
        {
            if (TestConfiguration.Options?.DbProvider == "Sqlite")
            {
                return _sqlite?.ConnectionString ?? throw new InvalidOperationException("Database has not been initialized");
            }

            // sql server
            if (_useContainerSqlServer)
            {
                return _container?.ConnectionString ?? throw new InvalidOperationException("Database has not been initialized");
            }
            return _localDb?.ConnectionString ?? throw new InvalidOperationException("Database has not been initialized");
        }

    }

    public async ValueTask InitializeAsync()
    {
        if (TestConfiguration.Options?.DbProvider == "Sqlite")
        {
            Console.WriteLine($"Using localdb database");
            _sqlite = new();
            await _sqlite.InitializeAsync();
        }
        else // sql server
        {
            var apiUserPassword = TestConfiguration.Options?.Databases?.Desic?.ApiUserPassword ?? throw new InvalidOperationException($"{nameof(IntegrationTestsDatabaseDesicOptions.ApiUserPassword)} is not configured");
            if (_useContainerSqlServer)
            {
                var image = TestConfiguration.Options?.DbProviders?.SqlServer?.ContainerImage ?? throw new InvalidOperationException($"Container image for sql server is not configured");
                Console.WriteLine($"Using container database");
                _container = new(image: image, apiUserPassword: apiUserPassword);
                await _container.InitializeAsync();
            }
            else
            {
                Console.WriteLine($"Using localdb database");
                _localDb = new(apiUserPassword: apiUserPassword);
                await _localDb.InitializeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _container?.DisposeAsync().AsTask().Wait();
        _localDb?.DisposeAsync().AsTask().Wait();
        _sqlite?.DisposeAsync().AsTask().Wait();
    }
}
