using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.Models.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.EntityFrameworkCore.Sqlite;

public static class ServiceCollectionHelpers
{
    public static void ConfigureDesicContextForSqlite(this IServiceCollection services, string? connectionString, bool setMigrationsAssembly = false, bool useSeeding = false)
    {
        services.AddDbContext<DesicContext>((serviceProvider, options) =>
        {
            options.UseSqlite(connectionString, x =>
            {
                if (setMigrationsAssembly) x.MigrationsAssembly(typeof(IMarker).Assembly.GetName().Name);
            });
            if (useSeeding) options.UseDesicContextSeeding(serviceProvider);
        });
    }
}
