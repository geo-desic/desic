using Microsoft.Extensions.Configuration;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

public sealed class DbFixture : IAsyncLifetime
{
    private DesicContextMsSqlContainer? _container;
    private DesicContextLocalDb? _localDb;
    private readonly bool _useContainer = TestSettingsConfiguration.Root.GetValue("Databases:Desic:UseContainer", false);

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
        if (_useContainer)
        {
            Console.WriteLine($"Using container database");
            _container = new DesicContextMsSqlContainer();
            await _container.InitializeAsync();
        }
        else
        {
            Console.WriteLine($"Using localdb database");
            _localDb = new DesicContextLocalDb();
            await _localDb.InitializeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _container?.DisposeAsync().AsTask().Wait();
        _localDb?.DisposeAsync().AsTask().Wait();
    }
}
