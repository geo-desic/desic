using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Desic.Infrastructure.Data;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseApplicationDbContextSeeding(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContextSeeder>>();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        ApplicationDbContextSeedingOptions bind = new();
        config.GetSection(ApplicationDbContextSeedingOptions.SectionName)?.Bind(bind);
        var seedingOptions = Options.Create(bind);
        options.UseSeeding((context, seed) =>
        {
            new ApplicationDbContextSeeder(context: (ApplicationDbContext)context, seed: seed, seedingOptions: seedingOptions, logger: logger, mediator: mediator).Apply();
        });
        options.UseAsyncSeeding(async (context, seed, cancellationToken) =>
        {
            await new ApplicationDbContextSeeder(context: (ApplicationDbContext)context, seed: seed, seedingOptions: seedingOptions, logger: logger, mediator: mediator).ApplyAsync(cancellationToken);
        });
        return options;
    }
}
