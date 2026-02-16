using Desic.EntityFrameworkCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desic.EntityFrameworkCore.Models.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDesicContextSeeding(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DesicContextSeeder>();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        DesicContextSeedingOptions bind = new();
        config.GetSection(DesicContextSeedingOptions.SectionName)?.Bind(bind);
        var seedingOptions = Options.Create(bind);
        options.UseSeeding((context, seed) =>
        {
            new DesicContextSeeder(context: (DesicContext)context, seed: seed, seedingOptions: seedingOptions, logger: logger, mediator: mediator).Apply();
        });
        options.UseAsyncSeeding(async (context, seed, cancellationToken) =>
        {
            await new DesicContextSeeder(context: (DesicContext)context, seed: seed, seedingOptions: seedingOptions, logger: logger, mediator: mediator).ApplyAsync(cancellationToken);
        });
        return options;
    }
}
