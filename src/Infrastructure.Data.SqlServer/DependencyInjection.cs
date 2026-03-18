using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.SqlServer;

public static class DependencyInjection
{
    public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration config, string? connectionString = null)
    {
        services.AddOptions<InitializeApplicationDatabaseOptions>().BindConfiguration(ConfigKeys.SectionInitialization).ValidateDataAnnotations().ValidateOnStart();
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyReference>())
            .ConfigureApplicationDbContextForSqlServer(connectionString: connectionString ?? config.GetValue("connection", config.GetConnectionString(DbProviders.SqlServer)), setMigrationsAssembly: config.GetValue(ApplicationDatabaseConfigKeys.MigrationsEnabled, true), useSeeding: config.GetValue(ApplicationDatabaseConfigKeys.SeedingEnabled, false));
        return services;
    }
}
