using Desic.Api.BackgroundServices;
using Desic.Api.Db;
using Desic.Api.HealthChecks;
using Desic.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<DesicContext>(options =>
{
    Providers.Configure(config, options);
    if (config.GetValue("DesicContext:EnableSensitiveDataLogging", false))
    {
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddHostedService<StartupBackgroundService>();
builder.Services.AddSingleton<StartupHealthCheck>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck("Alive", x => HealthCheckResult.Healthy())
    .AddDbContextCheck<DesicContext>(tags: ["ready"])
    .AddCheck<StartupHealthCheck>("Startup", tags: ["ready"]);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([typeof(Desic.Business.Marker).Assembly, typeof(Desic.EntityFrameworkCore.Marker).Assembly]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("v1/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("v1/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("v1/healthz/report", new HealthCheckOptions
{
    ResponseWriter = ResponseWriter.Write
});

app.MapControllers();

app.Run();
