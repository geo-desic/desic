using Desic.Api.HealthChecks;
using System.Diagnostics;

namespace Desic.Api.BackgroundServices;

public class StartupBackgroundService(IConfiguration config, IWebHostEnvironment environment, IServiceScopeFactory serviceScopeFactory, StartupHealthCheck startupHealthCheck) : BackgroundService
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly StartupHealthCheck _startupHealthCheck = startupHealthCheck ?? throw new ArgumentNullException(nameof(startupHealthCheck));

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var scope = _serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<StartupBackgroundService>>();
        logger.LogInformation("Starting {serviceName}", nameof(StartupBackgroundService));

        // any potential time consuming startup tasks

        _startupHealthCheck.StartupCompleted = true;
        // the startup health check can be used to determine when startup has completed

        stopwatch.Stop();
        logger.LogInformation("Completed {serviceName} in {elapsedTotalMilliseconds}ms", nameof(StartupBackgroundService), stopwatch.Elapsed.TotalMilliseconds);
    }
}
