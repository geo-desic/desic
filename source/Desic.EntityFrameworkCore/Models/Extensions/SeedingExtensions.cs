using Desic.EntityFrameworkCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Models.Extensions;

public static class SeedingExtensions
{
    public static DbContextOptionsBuilder UseDesicContextSeeding(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DesicContextSeeder>();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        options.UseSeeding((context, seed) =>
        {
            new DesicContextSeeder(context: (DesicContext)context, config: config, logger: logger, mediator: mediator).Apply();
        });
        options.UseAsyncSeeding(async (context, seed, cancellationToken) =>
        {
            await new DesicContextSeeder(context: (DesicContext)context, config: config, logger: logger, mediator: mediator).ApplyAsync(cancellationToken);
        });
        return options;
    }
}
