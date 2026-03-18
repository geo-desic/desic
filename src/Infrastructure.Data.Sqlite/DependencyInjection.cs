using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.Sqlite;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public static class DependencyInjection
{
    public static IServiceCollection AddSqliteInfrastructure(this IServiceCollection services, IConfiguration config, string? connectionString = null)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyReference>())
            .ConfigureApplicationDbContextForSqlite(connectionString: connectionString ?? config.GetValue("connection", config.GetConnectionString(DbProviders.Sqlite)), setMigrationsAssembly: config.GetValue(ApplicationDatabaseConfigKeys.MigrationsEnabled, true), useSeeding: config.GetValue(ApplicationDatabaseConfigKeys.SeedingEnabled, false));
}
