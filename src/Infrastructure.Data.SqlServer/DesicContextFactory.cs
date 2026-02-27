using Desic.Domain;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.Infrastructure.Data.SqlServer;

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
            var initialConfig = new JsonConfigurationSource() { Path = "sqlserver.appsettings.json", Optional = true };
            config.Sources.Insert(0, initialConfig);
        });
        result.ConfigureServices((hostContext, services) =>
        {
            services
                .AddDomain()
                .AddInfrastructure()
                .AddSqlServerInfrastructure(hostContext.Configuration);
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
