using Desic.DatabaseInitializer;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<WorkerService>();
builder.Services.AddTransient<DatabaseInitializer>();

builder.Services.Configure<DatabaseInitializerOptions>(builder.Configuration.GetSection(key: "Databases:Desic:SqlServer"));

using IHost host = builder.Build();

await host.RunAsync();