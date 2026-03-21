using Desic.Testing.Integration;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Hosting;
using Microsoft.Extensions.Configuration;

namespace Desic.Application.Tests.Integration;

public class TestHostDependencyTests(SeededAppDatabase testDatabase) : IAsyncLifetime
{
    private readonly string _connectionString = testDatabase?.GetConnectionString() ?? throw new ArgumentNullException(nameof(testDatabase));
    private readonly DbProvider _dbProvider = testDatabase?.DbProvider ?? throw new ArgumentNullException(nameof(testDatabase));
    private readonly TestHost _host = new();

    public IConfiguration Configuration => _host.Configuration;
    public IServiceProvider ServiceProvider => _host.ServiceProvider;

    public async ValueTask InitializeAsync()
    {
        await _host.InitializeAsync(new TestHostInitializationSettings { ConnectionString = _connectionString, DbProvider = _dbProvider });
    }

    public async ValueTask DisposeAsync()
    {
        await _host.DisposeAsync();
    }
}
