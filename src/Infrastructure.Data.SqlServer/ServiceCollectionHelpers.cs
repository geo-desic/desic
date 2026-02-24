using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.SqlServer;

public static class ServiceCollectionHelpers
{
    public static IServiceCollection ConfigureDesicContextForSqlServer(this IServiceCollection services, string? connectionString, bool setMigrationsAssembly = false, bool useSeeding = false)
    {
        services.AddDbContext<DesicContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, x =>
            {
                if (setMigrationsAssembly) x.MigrationsAssembly(typeof(IMarker).Assembly.GetName().Name);
            });
            if (useSeeding) options.UseDesicContextSeeding(serviceProvider);
        });
        return services;
    }

    public static IServiceCollection UseDatabaseInitializer(this IServiceCollection services, IConfiguration config, string configSectionKey = "Databases:Desic:SqlServer")
    {
        services.Configure<DatabaseInitializerOptions>(config.GetSection(key: configSectionKey));
        services.AddTransient<DatabaseInitializer>();
        return services;
    }
}
