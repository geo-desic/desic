using Desic.Infrastructure.Data;
using Desic.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.Infrastructure.SqlServer;

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
            config.AddJsonFile("sqlserver.appsettings.json", optional: true);
        });
        result.ConfigureServices((hostContext, services) =>
        {
            var config = hostContext.Configuration;
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(Domain.IMarker).Assembly, typeof(Infrastructure.IMarker).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            var connectionString = config.GetValue("connection", config.GetConnectionString("SqlServer"));
            services.ConfigureDesicContextForSqlServer(connectionString: connectionString, setMigrationsAssembly: true, useSeeding: true);
            services.UseDatabaseInitializer(config);
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
