using Desic.Application.Common.Interfaces;
using Desic.Infrastructure.Data;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<SeedApplicationDatabaseOptions>().BindConfiguration(ApplicationDatabaseConfigKeys.SectionSeeding);
        services
            .AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>())
            .AddDispatchR(typeof(IAssemblyReference).Assembly);
        return services;
    }
}
