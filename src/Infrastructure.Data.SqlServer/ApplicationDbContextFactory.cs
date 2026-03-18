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

    public static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var result = Host.CreateApplicationBuilder(args);
        result.Configuration.Sources.Insert(0, new JsonConfigurationSource() { Path = JsonConfigurationSources.AppSettingsFileNameSqlServer, Optional = true });
        if ((new string[] { "Test", "Local" }).Contains(result.Environment.EnvironmentName)) // Development already covered by Host.CreateDefaultBuilder above
        {
            result.Configuration.AddUserSecrets<IAssemblyReference>();
            // re-apply these so they will potentially override the user secrets (see the exact order in Host.CreateDefaultBuilder above)
            result.Configuration.AddEnvironmentVariables();
            result.Configuration.AddCommandLine(args);
        }
        result.Services
            .AddDomain()
            .AddInfrastructure()
            .AddSqlServerInfrastructure(result.Configuration);
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
