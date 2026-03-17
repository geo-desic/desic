using Desic.Shared.Mediator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<IAssemblyReference>(); cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)); })
            .AddValidatorsFromAssemblyContaining<IAssemblyReference>();
}
