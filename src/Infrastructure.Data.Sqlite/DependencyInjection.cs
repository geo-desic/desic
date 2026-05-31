using DispatchR.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.Sqlite;

public static class DependencyInjection
{
    public static IServiceCollection AddSqliteInfrastructure(this IServiceCollection services, IConfiguration config, string? connectionString = null)
        => services
            .AddDispatchR(typeof(IAssemblyReference).Assembly)
            .ConfigureApplicationDbContextForSqlite(connectionString: connectionString ?? config.GetValue("connection", config.GetConnectionString(DbProviders.Sqlite)), setMigrationsAssembly: config.GetValue(ApplicationDatabaseConfigKeys.MigrationsEnabled, true), useSeeding: config.GetValue(ApplicationDatabaseConfigKeys.SeedingEnabled, false));
}
