using Desic.Core.Mediator;
using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.Models.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desic.EntityFrameworkCore.Sqlite;

public class DesicContextFactory : IDisposable, IDesignTimeDbContextFactory<DesicContext>
{
    private bool _disposed = false;
    private const string ConfigKeyBase = "Databases:Desic";
    private const string ConfigKeyBaseLogging = "Logging:LogLevel";
    private const string ConfigKeySeeding = $"{ConfigKeyBase}:Seeding";
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
            var inMemoryConfig = new Dictionary<string, string?>
            {
                [$"{ConfigKeySeeding}:Enabled"] = "true",
                [$"{ConfigKeySeeding}:Users:Enabled"] = "true",
                [$"{ConfigKeySeeding}:Users:Test:Enabled"] = "true",
                [$"{ConfigKeySeeding}:Users:Test:Count"] = "5",
                [$"{ConfigKeySeeding}:Iso3166Countries:Enabled"] = "true",
                [$"{ConfigKeyBaseLogging}:Microsoft.AspNetCore"] = "Warning",
                [$"{ConfigKeyBaseLogging}:Microsoft.EntityFrameworkCore"] = "Warning",
            };
            config.AddInMemoryCollection(inMemoryConfig);
        });
        result.ConfigureServices((hostContext, services) =>
        {
            var config = hostContext.Configuration;
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(EntityFrameworkCore.Marker).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            var connectionString = config.GetValue("connection", config.GetConnectionString("Sqlite"));
            services.AddDbContext<DesicContext>(
                (serviceProvider, options) =>
                {
                    options.UseSqlite(connectionString, x => x.MigrationsAssembly(typeof(Marker).Assembly.GetName().Name));
                    options.UseDesicContextSeeding(serviceProvider);
                });
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
