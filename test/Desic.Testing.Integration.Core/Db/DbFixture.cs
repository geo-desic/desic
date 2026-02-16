using Xunit;

namespace Desic.Testing.Integration.Core.Db;

public sealed class DbFixture : IAsyncLifetime
{
    private DesicContextMsSqlContainer? _container;
    private DesicContextLocalDb? _localDb;
    private readonly bool _useContainer = TestConfiguration.Options?.Databases?.Desic?.UseContainer ?? false;

    public string ConnectionStringApp
    {
        get
        {
            if (_useContainer)
            {
                return _container?.ConnectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionStringApp)} has not been initialized for container database");
            }
            else
            {
                return _localDb?.ConnectionStringApp ?? throw new InvalidOperationException($"{nameof(ConnectionStringApp)} has not been initialized for localdb database");
            }
        }

    }
    public string ConnectionStringMigrations
    {
        get
        {
            if (_useContainer)
            {
                return _container?.ConnectionStringMigrations ?? throw new InvalidOperationException($"{nameof(ConnectionStringMigrations)} has not been initialized for container database");
            }
            else
            {
                return _localDb?.ConnectionStringMigrations ?? throw new InvalidOperationException($"{nameof(ConnectionStringMigrations)} has not been initialized for localdb database");
            }
        }
    }

    public async ValueTask InitializeAsync()
    {
        var appUserPassword = TestConfiguration.Options?.Databases?.Desic?.AppUserPassword ?? throw new InvalidOperationException($"App user password is not configured");
        if (_useContainer)
        {
            var image = TestConfiguration.Options?.Containers?.MsSql?.Image ?? throw new InvalidOperationException($"Container image for SQL Server is not configured");
            Console.WriteLine($"Using container database");
            _container = new DesicContextMsSqlContainer(image: image, appUserPassword: appUserPassword);
            await _container.InitializeAsync();
        }
        else
        {
            Console.WriteLine($"Using localdb database");
            _localDb = new DesicContextLocalDb(appUserPassword: appUserPassword);
            await _localDb.InitializeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _container?.DisposeAsync().AsTask().Wait();
        _localDb?.DisposeAsync().AsTask().Wait();
    }
}
