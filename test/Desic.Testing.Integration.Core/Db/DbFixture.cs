using Xunit;

namespace Desic.Testing.Integration.Core.Db;

public sealed class DbFixture : IAsyncLifetime
{
    private DesicContextMsSqlContainer? _container;
    private DesicContextLocalDb? _localDb;
    private DesicContextSqlite? _sqlite;
    private readonly bool _useContainer = TestConfiguration.Options?.Databases?.UseContainer ?? false;

    public string ConnectionStringApp
    {
        get
        {
            if (TestConfiguration.Options?.DbProvider == "Sqlite")
            {
                return _sqlite?.ConnectionString ?? throw new InvalidOperationException($"{nameof(_sqlite.ConnectionString)} has not been initialized for localdb database");
            }

            // sql server
            if (_useContainer)
            {
                return _container?.ConnectionStringApp ?? throw new InvalidOperationException($"{nameof(_container.ConnectionStringApp)} has not been initialized for container database");
            }
            return _localDb?.ConnectionStringApp ?? throw new InvalidOperationException($"{nameof(_localDb.ConnectionStringApp)} has not been initialized for localdb database");
        }

    }
    public string ConnectionStringMigrations
    {
        get
        {
            if (TestConfiguration.Options?.DbProvider == "Sqlite")
            {
                return _sqlite?.ConnectionString ?? throw new InvalidOperationException($"{nameof(_sqlite.ConnectionString)} has not been initialized for localdb database");
            }

            // sql server
            if (_useContainer)
            {
                return _container?.ConnectionStringMigrations ?? throw new InvalidOperationException($"{nameof(_container.ConnectionStringMigrations)} has not been initialized for container database");
            }
            return _localDb?.ConnectionStringMigrations ?? throw new InvalidOperationException($"{nameof(_localDb.ConnectionStringMigrations)} has not been initialized for localdb database");
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
            var appUserPassword = TestConfiguration.Options?.Databases?.Desic?.AppUserPassword ?? throw new InvalidOperationException($"{nameof(IntegrationTestsDatabaseDesicOptions.AppUserPassword)} is not configured");
            if (_useContainer)
            {
                var image = TestConfiguration.Options?.Containers?.MsSql?.Image ?? throw new InvalidOperationException($"Container image for sql server is not configured");
                Console.WriteLine($"Using container database");
                _container = new(image: image, appUserPassword: appUserPassword);
                await _container.InitializeAsync();
            }
            else
            {
                Console.WriteLine($"Using localdb database");
                _localDb = new(appUserPassword: appUserPassword);
                await _localDb.InitializeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _container?.DisposeAsync().AsTask().Wait();
        _localDb?.DisposeAsync().AsTask().Wait();
    }
}
