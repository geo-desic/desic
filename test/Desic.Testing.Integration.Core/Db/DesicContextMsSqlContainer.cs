using Desic.EntityFrameworkCore.Models;
using Desic.Testing.Integration.Core.WebApplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

namespace Desic.Testing.Integration.Core.Db;

// class is sealed for simper IAsyncLifetime implementation
// see https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#sealed-alternative-async-dispose-pattern
public sealed class DesicContextMsSqlContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder(TestSettingsConfiguration.Root.GetValue("Containers:MsSql:Image", string.Empty)).Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        Console.WriteLine($"Starting mssql container");
        await _container.StartAsync();
        Console.WriteLine($"Started mssql container with name = {_container.Name} and id = {_container.Id} and image = {_container.Image.FullName}");

        Console.WriteLine($"Attempting to instantiate a DesicContext");
        using var factory = new TestWebApplicationFactory<Program>(ConnectionString);
        var serviceProvider = factory.Services;
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DesicContext>();
        Console.WriteLine($"Successfully instantiated a DesicContext");

        Console.WriteLine($"Attempting to initialize the database");
        using var cts = new CancellationTokenSource();
        await context.Database.MigrateAsync(cts.Token);
        Console.WriteLine($"Successfully initialized the database");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
