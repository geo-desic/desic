using Desic.Domain;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.Infrastructure.Data.SqlServer;

public sealed class ApplicationDbContextFactory : IDisposable, IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private bool _disposed = false;
    private IHost? _host;
    private IServiceScope? _scope;

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);
        _host = hostBuilder.Build();
        _scope = _host.Services.CreateScope();
        var result = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return result;
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var result = Host.CreateDefaultBuilder(args);
        result.ConfigureAppConfiguration((hostContext, config) =>
        {
            var initialConfig = new JsonConfigurationSource() { Path = "sqlserver.appsettings.json", Optional = true };
            config.Sources.Insert(0, initialConfig);
            // for adding user secrets in a couple more environments
            if ((new string[] { "Test", "Local" }).Contains(hostContext.HostingEnvironment.EnvironmentName)) // Development already covered by Host.CreateDefaultBuilder above
            {
                config.AddUserSecrets<IMarker>();
                // re-apply these so they will potentially override the user secrets (see the exact order in Host.CreateDefaultBuilder above)
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            }
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
