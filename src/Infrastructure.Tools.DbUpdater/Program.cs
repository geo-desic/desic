using Desic.DatabaseUpdater;
using Desic.Domain;
using Desic.Infrastructure;
using Desic.Infrastructure.Data.Sqlite;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
var config = builder.Configuration;
config.Sources.Insert(0, new JsonConfigurationSource() { Path = "sqlserver.appsettings.json", Optional = true });
config.Sources.Insert(0, new JsonConfigurationSource() { Path = "sqlite.appsettings.json", Optional = true });
config.AddCommandLine(args);

var dbProvider = config.GetValue("provider", config.GetValue<string>("DbProvider") ?? throw new InvalidOperationException("Database provider could not be determined"));

builder.Services.AddHostedService<WorkerService>();

var connectionStringMigrations = config.GetValue<string>("c") ?? config.GetValue<string>("connection");
var connectionStringInitialization = config.GetValue<string>("ci") ?? config.GetValue<string>("connection-init");
var useSeeding = !(args.Contains("--ns") || args.Contains("--no-seeding"));

if (connectionStringMigrations != null) builder.Services.AddDomain().AddInfrastructure();

switch (dbProvider)
{
    case "Sqlite":
        if (connectionStringMigrations != null) builder.Services.ConfigureApplicationDbContextForSqlite(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    case "SqlServer":
        if (connectionStringInitialization != null) builder.Services.UseDatabaseInitializer(config);
        if (connectionStringMigrations != null) builder.Services.ConfigureApplicationDbContextForSqlServer(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    default:
        throw new NotSupportedException($"Unsupported database provider: {dbProvider}");
}

using IHost host = builder.Build();

await host.RunAsync();