using Desic.Api.BackgroundServices;
using Desic.Api.Db;
using Desic.Api.HealthChecks;
using Desic.Business.Users.Validators;
using Desic.EntityFrameworkCore.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

ILogger logger = loggerFactory.CreateLogger("BeforeAppBuild");

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

logger.LogDebug("Starting configuration of web application builder services");

// Add services to the container.
builder.Services.AddDbContext<DesicContext>(options =>
{
    Providers.Configure(config, options, logger);
    if (config.GetValue("DesicContext:EnableSensitiveDataLogging", false))
    {
        logger.LogWarning("Enabling sensitive data logging");
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
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

var app = builder.Build();

logger.LogDebug("Completed configuration of web application builder services");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    logger.LogWarning("Configuring the app for swagger support");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

logger.LogDebug("Adding v1/healthz/ endpoints");

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

logger.LogDebug("Mapping app controllers");
app.MapControllers();

app.Run();
