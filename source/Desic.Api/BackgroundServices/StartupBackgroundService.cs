using Desic.Api.HealthChecks;
using Desic.EntityFrameworkCore.Models;

namespace Desic.Api.BackgroundServices
{
    public class StartupBackgroundService(IWebHostEnvironment environment, IServiceScopeFactory serviceScopeFactory, StartupHealthCheck startupHealthCheck) : BackgroundService
    {
        private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        private readonly StartupHealthCheck _startupHealthCheck = startupHealthCheck ?? throw new ArgumentNullException(nameof(startupHealthCheck));

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_environment.IsDevelopment())
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DesicContext>();
                await DesicContext.InitializeAsync(context, cancellationToken);
            }

            _startupHealthCheck.StartupCompleted = true;
        }
    }
}
