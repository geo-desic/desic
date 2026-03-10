using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Desic.Infrastructure.Tools.DbUpdater;

public class WorkerService(IServiceProvider serviceProvider, IConfiguration config, ILogger<WorkerService> logger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private int? _exitCode;
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    private readonly ILogger<WorkerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var initilizationPerformed = false;
        var migrationsPerformed = false;

        var dbProvider = _config.GetValue<string>(ConfigKeys.DbProvider);
        if (dbProvider == null)
        {
            _logger.LogError("Database provider could not be determined");
            StopApplication(exitCode: 1);
            return;
        }

        // initialization
        var connectionStringInitialization = _config.GetValue<string>(ConfigKeys.ConnectionStringInitialization);
        if (connectionStringInitialization != null)
        {
            initilizationPerformed = await PerformInitialization(dbProvider: dbProvider, connectionString: connectionStringInitialization, cancellationToken: stoppingToken);
        }
        else
        {
            _logger.LogInformation("Skipping initialization as no connection string for it was provided");
        }

        // migrations
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (context != null)
            {
                await context.Database.MigrateAsync(stoppingToken);
                migrationsPerformed = true;
            }
        }

        if (!initilizationPerformed && !migrationsPerformed)
        {
            _logger.LogError("No initialization or migrations were performed because no connection strings were provided for them");
            StopApplication(exitCode: 1);
            return;
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

    private async Task<bool> PerformInitialization(string dbProvider, string connectionString, CancellationToken cancellationToken)
    {
        if (dbProvider == "SqlServer")
        {
            if (connectionString == "migrations")
            {
                var connectionStringMigrations = _config.GetValue<string>(ConfigKeys.ConnectionStringMigrations);
                if (connectionStringMigrations == null)
                {
                    _logger.LogError("Connection string for initialization is 'migrations' but no connection string for migrations was provided");
                    StopApplication(exitCode: 1);
                    return false;
                }
                connectionString = connectionStringMigrations;
                _logger.LogDebug("Using migrations connection string for initialization");
            }

            using var scope = _serviceProvider.CreateScope();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await databaseInitializer.InitializeAsync(connectionString: connectionString, cancellationToken: cancellationToken);
            return true;
        }
        else
        {
            _logger.LogWarning("Initialization will be skipped as the pecified database provider does not currently support it");
        }
        return false;
    }
}