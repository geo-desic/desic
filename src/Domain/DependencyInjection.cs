using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
        => services
            .AddValidatorsFromAssemblyContaining<IAssemblyReference>();
}
