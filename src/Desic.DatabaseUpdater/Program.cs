using Desic.DatabaseUpdater;
using Desic.Infrastructure.Data.Sqlite;
using Desic.Infrastructure.SqlServer;
using Desic.Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilderSettings settings = new()
{
    Args = args,
    Configuration = new ConfigurationManager(),
    ContentRootPath = Directory.GetCurrentDirectory(),
};

settings.Configuration.AddJsonFile("sqlite.appsettings.json", optional: true);
settings.Configuration.AddJsonFile("sqlserver.appsettings.json", optional: true);
settings.Configuration.AddJsonFile("appsettings.json", optional: true);
settings.Configuration.AddJsonFile($"appsettings.{settings.EnvironmentName}.json", optional: true);
settings.Configuration.AddUserSecrets<Program>(optional: true);
settings.Configuration.AddEnvironmentVariables();
settings.Configuration.AddCommandLine(args);

HostApplicationBuilder builder = Host.CreateApplicationBuilder(settings);

var config = builder.Configuration;
var dbProvider = config.GetValue("provider", config.GetValue<string>("DbProvider") ?? throw new InvalidOperationException("Database provider could not be determined"));

builder.Services.AddHostedService<WorkerService>();

var connectionStringMigrations = config.GetValue<string>("c") ?? config.GetValue<string>("connection");
var connectionStringInitialization = config.GetValue<string>("ci") ?? config.GetValue<string>("connection-init");
var useSeeding = !(args.Contains("--ns") || args.Contains("--no-seeding"));

if (connectionStringMigrations != null)
{
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(typeof(Desic.Domain.IMarker).Assembly, typeof(Desic.Infrastructure.IMarker).Assembly);
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