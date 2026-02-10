using Desic.Api.BackgroundServices;
using Desic.Api.Db;
using Desic.Api.HealthChecks;
using Desic.Business.Users.Models.Validators;
using Desic.Core.Mediator;
using Desic.EntityFrameworkCore.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

ILogger logger = loggerFactory.CreateLogger("BeforeAppBuilt");

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var dbProvider = config.GetValue("DbProvider", Providers.SqlServer.Name)!;
logger.LogInformation("Database provider determined from configuration: {dbProvider}", dbProvider);

var httpLoggingEnabled = config.GetValue("HttpLogging:Enabled", false);
logger.LogInformation("Http logging enabled value determined from configuration: {httpLoggingEnabled}", httpLoggingEnabled);

logger.LogInformation("Starting configuration of the web application builder");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddDbContext<DesicContext>(options =>
{
    Providers.Configure(config, options, dbProvider);
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies([typeof(Desic.Business.Marker).Assembly, typeof(Desic.EntityFrameworkCore.Marker).Assembly]);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();
if (httpLoggingEnabled)
{
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders | HttpLoggingFields.Duration;
        logging.CombineLogs = true;
    });
}

logger.LogInformation("Completed configuration of the web application builder");

var app = builder.Build();

app.Logger.LogInformation("Built the web application");

var isDevelopment = app.Environment.IsDevelopment();
app.Logger.LogInformation("Application environment: {appEnvironmentName}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Application is development: {appIsDevelopment}", isDevelopment);

if (httpLoggingEnabled)
{
    app.Logger.LogInformation("Configuring the app to use http logging");
    app.UseHttpLogging();
}

if (isDevelopment)
{
    app.Logger.LogInformation("Configuring the app for swagger support");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Logger.LogInformation("Configuring the app to use https redirection");
app.UseHttpsRedirection();

//app.Logger.LogInformation("Configuring the app to use authentication");
//app.UseAuthentication();

app.Logger.LogInformation("Configuring the app to use authorization");
app.UseAuthorization();

var endpoint = "v1/healthz/live";
app.Logger.LogInformation($"Configuring the endpoint: {endpoint}");
app.MapHealthChecks(endpoint, new HealthCheckOptions
{
    Predicate = _ => false
});

endpoint = "v1/healthz/ready";
app.Logger.LogInformation($"Configuring the endpoint: {endpoint}");
app.MapHealthChecks(endpoint, new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

endpoint = "v1/healthz/report";
app.Logger.LogInformation($"Configuring the endpoint: {endpoint}");
app.MapHealthChecks(endpoint, new HealthCheckOptions
{
    ResponseWriter = ResponseWriter.Write
});

app.Logger.LogInformation("Configuring the app to use controllers");
app.MapControllers();

app.Run();
