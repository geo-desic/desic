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
        var initializationEnabled = config.GetValue(Data.SqlServer.ConfigKeys.InitializationEnabled, false);
        if (initializationEnabled)
        {
            var connectionStringInitialization = _config.GetConnectionStringInitialization();
            initilizationPerformed = await PerformInitialization(dbProvider: dbProvider, connectionString: connectionStringInitialization, cancellationToken: stoppingToken);
        }
        else
        {
            _logger.LogInformation("Skipping initialization because it is not enabled");
        }

        // migrations
        var migrationsEnabled = config.GetValue(ConfigKeys.MigrationsEnabled, false);
        if (migrationsEnabled)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(stoppingToken);
            migrationsPerformed = true;
        }
        else
        {
            _logger.LogInformation("Skipping migrations because it is not enabled");
        }

        if (!initilizationPerformed && !migrationsPerformed)
        {
            _logger.LogError("No initialization or migrations occured because they are not enabled");
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
        if (dbProvider == DbProviders.SqlServer)
        {
            if (connectionString == "migrations")
            {
                connectionString = _config.GetConnectionStringMigrations();
                if (connectionString == null)
                {
                    _logger.LogError("Connection string for initialization is 'migrations' but no connection string for migrations could be determined");
                    StopApplication(exitCode: 1);
                    return false;
                }
                _logger.LogDebug("Using migrations connection string for initialization");
            }

            using var scope = _serviceProvider.CreateScope();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await databaseInitializer.InitializeAsync(connectionString: connectionString, cancellationToken: cancellationToken);
            return true;
        }
        else
        {
            _logger.LogWarning("Initialization will be skipped as the specified database provider does not currently support it");
        }
        return false;
    }
}