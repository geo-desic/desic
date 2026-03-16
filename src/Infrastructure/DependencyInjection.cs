using Desic.Application.Common.Interfaces;
using Desic.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<ApplicationDbContextSeedingOptions>().BindConfiguration(ApplicationDbContextSeedingOptions.SectionName);
        services
            .AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>())
            .AddTransient<ApplicationDbContextSeeder>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>());
        return services;
    }
}
