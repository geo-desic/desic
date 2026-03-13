using Desic.Domain;
using Desic.Infrastructure;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Sqlite;
using Desic.Infrastructure.Data.SqlServer;
using Desic.Infrastructure.Tools.DbUpdater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
var config = builder.Configuration;
config.Sources.Insert(0, new JsonConfigurationSource() { Path = "sqlserver.appsettings.json", Optional = true });
config.Sources.Insert(0, new JsonConfigurationSource() { Path = "sqlite.appsettings.json", Optional = true });
var mappings = new Dictionary<string, string>
{
    { "-c", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.ConnectionStringMigrations },
    { "-ci", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.ConnectionStringInitialization },
    { "-ie", Desic.Infrastructure.Data.SqlServer.ConfigKeys.InitializationEnabled },
    { "--initialization-enabled", Desic.Infrastructure.Data.SqlServer.ConfigKeys.InitializationEnabled },
    { "-me", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.MigrationsEnabled },
    { "--migrations-enabled", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.MigrationsEnabled },
    { "-p", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider },
    { "--provider", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider },
    { "-s", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.SeedingEnabled },
    { "--seeding", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.SeedingEnabled },
};
config.AddCommandLine(args, mappings);

var dbProvider = config.GetValue<string>(Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider) ?? throw new InvalidOperationException("Database provider could not be determined");

builder.Services.AddHostedService<WorkerService>();

var connectionStringMigrations = config.GetConnectionStringMigrations();
var initializationEnabled = config.GetValue(Desic.Infrastructure.Data.SqlServer.ConfigKeys.InitializationEnabled, false);
var migrationsEnabled = config.GetValue(Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.MigrationsEnabled, false);
var useSeeding = config.GetValue(Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.SeedingEnabled, false);

if (connectionStringMigrations != null) builder.Services.AddDomain().AddInfrastructure();

switch (dbProvider)
{
    case DbProviders.Sqlite:
        if (migrationsEnabled) builder.Services.ConfigureApplicationDbContextForSqlite(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    case DbProviders.SqlServer:
        if (initializationEnabled) builder.Services.UseDatabaseInitializer(config);
        if (migrationsEnabled) builder.Services.ConfigureApplicationDbContextForSqlServer(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    default:
        throw new NotSupportedException($"Unsupported database provider: {dbProvider}");
}

using IHost host = builder.Build();

await host.RunAsync();