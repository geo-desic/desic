using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Desic.DatabaseUpdater;

public class WorkerService(IServiceProvider serviceProvider, IConfiguration config, ILogger<WorkerService> logger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private int? _exitCode;
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    private readonly ILogger<WorkerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var noInit = _config.GetValue("no-init", false);
        var noMigrate = _config.GetValue("no-migrate", false);

        if (noInit && noMigrate)
        {
            _logger.LogWarning("No initialization or migration performed as both 'no-init' and 'no-migrate' flags are set");
            StopApplication(exitCode: 0);
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var dbProvider = config.GetValue("provider", config.GetValue<string>("DbProvider"));
        if (dbProvider == null)
        {
            _logger.LogError("Database provider could not be determined");
            StopApplication(exitCode: 1);
            return;
        }

        if (!noInit && dbProvider == "SqlServer") // Sqlite does not require initialization only SqlServer
        {
            var connectionString = _config.GetValue<string>("connection") ?? _config.GetConnectionString("SqlServer");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("No connection string was provided");
                StopApplication(exitCode: 1);
                return;
            }

            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await databaseInitializer.InitializeAsync(connectionString: connectionString, cancellationToken: stoppingToken);
        }

        if (!noMigrate)
        {
            var context = scope.ServiceProvider.GetRequiredService<DesicContext>();
            await context.Database.MigrateAsync(stoppingToken);
        }

        StopApplication(exitCode: 0);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
    }

    private void StopApplication(int? exitCode = null)
    {
        if (exitCode.HasValue) _exitCode = exitCode.Value;
        _hostApplicationLifetime.StopApplication();
    }
}