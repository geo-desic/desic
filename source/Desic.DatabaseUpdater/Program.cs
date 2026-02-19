using Desic.Core.Mediator;
using Desic.DatabaseUpdater;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var dbProvider = config.GetValue("provider", config.GetValue<string>("DbProvider") ?? throw new InvalidOperationException("Database provider could not be determined"));

builder.Services.AddHostedService<WorkerService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Desic.EntityFrameworkCore.IMarker).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

string connectionString;

switch (dbProvider)
{
    case "Sqlite":
        connectionString = config.GetValue<string>("connection") ?? config.GetConnectionString("Sqlite") ?? throw new InvalidOperationException("Connection string could not be determined");
        builder.Services.ConfigureDesicContextForSqlServer(connectionString: connectionString, setMigrationsAssembly: true, useSeeding: true);
        break;
    case "SqlServer":
        connectionString = config.GetValue<string>("connection") ?? config.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("Connection string could not be determined");
        builder.Services.UseDatabaseInitializer(config);
        builder.Services.ConfigureDesicContextForSqlServer(connectionString: connectionString, setMigrationsAssembly: true, useSeeding: true);
        break;
    default:
        throw new NotSupportedException($"Unsupported database provider: {dbProvider}");
}

using IHost host = builder.Build();

await host.RunAsync();