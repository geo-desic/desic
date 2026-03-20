using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Hosting;
using Microsoft.Extensions.Hosting;

namespace Desic.Application.Tests.Integration;

public class TestHostDependencyTests(SeededAppDatabase testDatabase) : IAsyncLifetime
{
    private readonly TestHost _host = new(connectionString: testDatabase.GetConnectionString(), dbProvider: testDatabase.DbProvider);

    public IHost Host => _host.Host;

    public async ValueTask InitializeAsync()
    {
        await _host.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _host.DisposeAsync();
    }
}
