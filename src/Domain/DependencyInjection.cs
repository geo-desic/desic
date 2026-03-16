using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Domain;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
        => services
            .AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<IAssemblyReference>(); })
            .AddValidatorsFromAssemblyContaining<IAssemblyReference>();
}
