using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Sqlite;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddSqliteInfrastructure(this IServiceCollection services, IConfiguration config, string? connectionString = null)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>())
            .ConfigureDesicContextForSqlite(connectionString: connectionString ?? config.GetConnectionString("Sqlite"), setMigrationsAssembly: config.GetValue("Databases:Desic:Migrations:Enabled", false), useSeeding: config.GetValue("Databases:Desic:Seeding:Enabled", false));
}
