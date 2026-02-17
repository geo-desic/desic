using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Desic.DatabaseInitializer;

public class WorkerService(EntityFrameworkCore.SqlServer.DatabaseInitializer databaseInitializer, IConfiguration config, ILogger<WorkerService> logger, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly EntityFrameworkCore.SqlServer.DatabaseInitializer _databaseInitializer = databaseInitializer ?? throw new ArgumentNullException(nameof(databaseInitializer));
    private int? _exitCode;
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    private readonly ILogger<WorkerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionString = _config.GetConnectionString("connection") ?? _config.GetConnectionString("SqlServer");
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("No connection string was provided");
            _exitCode = 1;
            return;
        }

        await _databaseInitializer.InitializeAsync(connectionString, stoppingToken);

        _hostApplicationLifetime.StopApplication();

        _exitCode = 0;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
    }
}