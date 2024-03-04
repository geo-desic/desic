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
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DesicContext>(tags: ["ready"])
    .AddCheck("Alive", x => HealthCheckResult.Healthy());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DesicContext>();
    await DesicContext.InitializeAsync(db);
}

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
