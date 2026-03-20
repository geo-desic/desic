using Desic.Application;
using Desic.Domain;
using Desic.Infrastructure;
using Desic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.Testing.Integration.Hosting;

public class TestHost(string connectionString, DbProvider dbProvider) : IDisposable, IAsyncDisposable
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    private readonly DbProvider _dbProvider = dbProvider;
    private IHost? _host;

    public IHost Host => _host ?? throw new InvalidOperationException($"{nameof(Host)} has not been initialized yet");

    public async ValueTask InitializeAsync()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(["--environment", Constants.TestEnvironmentName]);
        builder.Services.AddDomain().AddApplication().AddInfrastructure();
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            _ = _dbProvider switch
            {
                DbProvider.Sqlite => options.UseSqlite(connectionString: _connectionString),
                DbProvider.SqlServer => options.UseSqlServer(connectionString: _connectionString),
                _ => throw new NotSupportedException($"Unsupported db provider: {_dbProvider}"),
            };
        });
        _host = builder.Build();

        await _host.StartAsync();
    }

    #region Dispoable
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _host?.Dispose();
            _host = null;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_host is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _host?.Dispose();
        }
        _host = null;
    }
    #endregion
}
