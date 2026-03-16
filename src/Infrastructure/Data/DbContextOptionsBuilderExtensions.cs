using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseApplicationDbContextSeeding(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        options.UseSeeding((context, seed) =>
        {
            var seeder = serviceProvider.GetRequiredService<ApplicationDbContextSeeder>();
            seeder.Apply();
        });
        options.UseAsyncSeeding(async (context, seed, cancellationToken) =>
        {
            var seeder = serviceProvider.GetRequiredService<ApplicationDbContextSeeder>();
            await seeder.ApplyAsync(cancellationToken);
        });
        return options;
    }
}
