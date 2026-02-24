using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>())
            .AddValidatorsFromAssemblyContaining<IMarker>();
}
