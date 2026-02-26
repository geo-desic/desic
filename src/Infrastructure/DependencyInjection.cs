using Desic.Application.Common.Interfaces;
using Desic.Domain.Common;
using Desic.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>())
            .AddScoped<IDesicContext>(provider => provider.GetRequiredService<DesicContext>())
            .AddScoped(typeof(IRepository<>), typeof(DesicRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(DesicRepository<>));
}
