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
    { "-me", ApplicationDatabaseConfigKeys.MigrationsEnabled },
    { "--migrations-enabled", ApplicationDatabaseConfigKeys.MigrationsEnabled },
    { "-p", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider },
    { "--provider", Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider },
    { "-s", ApplicationDatabaseConfigKeys.SeedingEnabled },
    { "--seeding", ApplicationDatabaseConfigKeys.SeedingEnabled },
};
config.AddCommandLine(args, mappings);

var dbProvider = config.GetValue<string>(Desic.Infrastructure.Tools.DbUpdater.ConfigKeys.DbProvider) ?? throw new InvalidOperationException("Database provider could not be determined");

builder.Services.AddDomain().AddInfrastructure();
builder.Services.AddHostedService<WorkerService>();

var connectionStringMigrations = config.GetConnectionStringMigrations();

switch (dbProvider)
{
    case DbProviders.Sqlite:
        builder.Services.AddSqliteInfrastructure(config: config, connectionString: connectionStringMigrations);
        break;
    case DbProviders.SqlServer:
        builder.Services.AddSqlServerInfrastructure(config: config, connectionString: connectionStringMigrations);
        break;
    default:
        throw new NotSupportedException($"Unsupported database provider: {dbProvider}");
}

using IHost host = builder.Build();

await host.RunAsync();