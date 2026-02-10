using Desic.Api.HealthChecks;
using Desic.EntityFrameworkCore.Models;

namespace Desic.Api.BackgroundServices;

public class StartupBackgroundService(IConfiguration config, IWebHostEnvironment environment, IServiceScopeFactory serviceScopeFactory, StartupHealthCheck startupHealthCheck) : BackgroundService
{
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly StartupHealthCheck _startupHealthCheck = startupHealthCheck ?? throw new ArgumentNullException(nameof(startupHealthCheck));

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<StartupBackgroundService>>();
        var allowInitialization = _config.GetValue("DesicContext:AllowInitialization", false);
        logger.LogDebug("Allow initialization of DesicContext: {desicContextAllowInit}", allowInitialization);

        if (allowInitialization && _environment.IsDevelopment())
        {
            var context = scope.ServiceProvider.GetRequiredService<DesicContext>();
            logger.LogInformation("Starting initialization of DesicContext");
            await DesicContext.InitializeAsync(context, cancellationToken);
            logger.LogInformation("Completed initialization of DesicContext");
        }
        else if (allowInitialization)
        {
            logger.LogWarning("DesicContext initialization is not allowed in non development web application environments");
        }

        _startupHealthCheck.StartupCompleted = true;
    }
}
