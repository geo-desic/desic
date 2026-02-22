using Desic.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.EntityFrameworkCore.SqlServer;

public static class ServiceCollectionHelpers
{
    public static void ConfigureDesicContextForSqlServer(this IServiceCollection services, string? connectionString, bool setMigrationsAssembly = false, bool useSeeding = false)
    {
        services.AddDbContext<DesicContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, x =>
            {
                if (setMigrationsAssembly) x.MigrationsAssembly(typeof(IMarker).Assembly.GetName().Name);
            });
            if (useSeeding) options.UseDesicContextSeeding(serviceProvider);
        });
    }

    public static void UseDatabaseInitializer(this IServiceCollection services, IConfiguration config, string configSectionKey = "Databases:Desic:SqlServer")
    {
        services.Configure<DatabaseInitializerOptions>(config.GetSection(key: configSectionKey));
        services.AddTransient<DatabaseInitializer>();
    }
}
