using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.SqlServer;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration config, string? connectionString = null)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IMarker>())
            .UseDatabaseInitializer(config)
            .ConfigureApplicationDbContextForSqlServer(connectionString: connectionString ?? config.GetValue("connection", config.GetConnectionString(DbProviders.SqlServer)), setMigrationsAssembly: config.GetValue("Databases:Application:Migrations:Enabled", true), useSeeding: config.GetValue("Databases:Application:Seeding:Enabled", false));
}
