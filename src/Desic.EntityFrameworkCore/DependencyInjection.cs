using Desic.Core.Shared;
using Desic.EntityFrameworkCore.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.EntityFrameworkCore;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>())
            .AddScoped(typeof(IRepository<>), typeof(DesicRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(DesicRepository<>));
}
