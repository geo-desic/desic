using Desic.Shared.Mediator;
using DispatchR.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddDispatchR(options =>
            {
                options.Assemblies.AddRange([typeof(IAssemblyReference).Assembly, typeof(Shared.IAssemblyReference).Assembly]);
                options.RegisterPipelines = true;
                options.PipelineOrder = [typeof(LoggingBehavior<,>)];
            })
            .AddValidatorsFromAssemblyContaining<IAssemblyReference>();
}
