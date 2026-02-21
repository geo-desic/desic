using Desic.EntityFrameworkCore.Models;
using Desic.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.EntityFrameworkCore.Sqlite;

public sealed class DesicContextFactory : IDisposable, IDesignTimeDbContextFactory<DesicContext>
{
    private bool _disposed = false;
    private IHost? _host;
    private IServiceScope? _scope;

    public DesicContext CreateDbContext(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);
        _host = hostBuilder.Build();
        _scope = _host.Services.CreateScope();
        var result = _scope.ServiceProvider.GetRequiredService<DesicContext>();
        return result;
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var result = Host.CreateDefaultBuilder(args);
        result.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("sqlite.appsettings.json", optional: true);
        });
        result.ConfigureServices((hostContext, services) =>
        {
            var config = hostContext.Configuration;
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(Desic.Data.IMarker).Assembly, typeof(EntityFrameworkCore.IMarker).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            var connectionString = config.GetValue("connection", config.GetConnectionString("Sqlite"));
            services.ConfigureDesicContextForSqlite(connectionString: connectionString, setMigrationsAssembly: true, useSeeding: true);
        });
        return result;
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _host?.Dispose();
            _scope?.Dispose();
            _host = null;
            _scope = null;
        }

        _disposed = true;
    }
    #endregion
}
