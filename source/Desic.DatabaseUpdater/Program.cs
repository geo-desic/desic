using Desic.Core.Mediator;
using Desic.DatabaseUpdater;
using Desic.EntityFrameworkCore.Sqlite;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var useSeeding = !(args.Contains("--ns") || args.Contains("--no-seeding"));

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var dbProvider = config.GetValue("provider", config.GetValue<string>("DbProvider") ?? throw new InvalidOperationException("Database provider could not be determined"));

builder.Services.AddHostedService<WorkerService>();

var connectionStringMigrations = config.GetValue<string>("c") ?? config.GetValue<string>("connection");
var connectionStringInitialization = config.GetValue<string>("ci") ?? config.GetValue<string>("connection-init");

if (connectionStringMigrations != null)
{
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Desic.EntityFrameworkCore.IMarker).Assembly);
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });
}

switch (dbProvider)
{
    case "Sqlite":
        if (connectionStringMigrations != null) builder.Services.ConfigureDesicContextForSqlite(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    case "SqlServer":
        if (connectionStringInitialization != null) builder.Services.UseDatabaseInitializer(config);
        if (connectionStringMigrations != null) builder.Services.ConfigureDesicContextForSqlServer(connectionString: connectionStringMigrations, setMigrationsAssembly: true, useSeeding: useSeeding);
        break;
    default:
        throw new NotSupportedException($"Unsupported database provider: {dbProvider}");
}

using IHost host = builder.Build();

await host.RunAsync();