using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.SqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationDbContextForSqlServer(this IServiceCollection services, string? connectionString, bool setMigrationsAssembly = false, bool useSeeding = false)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, x =>
            {
                if (setMigrationsAssembly) x.MigrationsAssembly(typeof(IAssemblyReference).Assembly.GetName().Name);
            });
            if (useSeeding) options.UseApplicationDbContextSeeding(serviceProvider);
        });
        return services;
    }
}
