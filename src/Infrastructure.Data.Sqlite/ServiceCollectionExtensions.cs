using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationDbContextForSqlite(this IServiceCollection services, string? connectionString, bool setMigrationsAssembly = false, bool useSeeding = false)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlite(connectionString, x =>
            {
                if (setMigrationsAssembly) x.MigrationsAssembly(typeof(IMarker).Assembly.GetName().Name);
            });
            if (useSeeding) options.UseApplicationDbContextSeeding(serviceProvider);
        });
        return services;
    }
}
