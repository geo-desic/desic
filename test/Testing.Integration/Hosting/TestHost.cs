using Desic.Application;
using Desic.Domain;
using Desic.Infrastructure;
using Desic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.Testing.Integration.Hosting;

public class TestHost : IDisposable, IAsyncDisposable
{
    private IHost? _host;

    public IConfiguration Configuration => _host?.Services.GetRequiredService<IConfiguration>() ?? throw HostNotInitializedException;
    public IServiceProvider ServiceProvider => _host?.Services ?? throw HostNotInitializedException;

    public async ValueTask InitializeAsync(TestHostInitializationSettings? settings = null)
    {
        settings ??= new();
        var builderSettings = new HostApplicationBuilderSettings
        {
            EnvironmentName = settings.EnvironmentName,
            Args = settings.CommandLineArgs,
        };
        var builder = Host.CreateApplicationBuilder(settings: builderSettings);
        builder.Services.AddDomain().AddApplication().AddInfrastructure();
        if (settings.ConnectionString != null && settings.DbProvider != null)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                _ = settings.DbProvider.Value switch
                {
                    DbProvider.Sqlite => options.UseSqlite(connectionString: settings.ConnectionString),
                    DbProvider.SqlServer => options.UseSqlServer(connectionString: settings.ConnectionString),
                    _ => throw new NotSupportedException($"Unsupported db provider: {settings.DbProvider}"),
                };
            });
        }
        settings.RegisterServices?.Invoke(builder.Services, builder.Configuration);
        _host = builder.Build();

        await _host.StartAsync();
    }

    private static InvalidOperationException HostNotInitializedException => new("Host has not been initialized");

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
