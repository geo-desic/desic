using Desic.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        var optionsBuilder = new DbContextOptionsBuilder<DesicContext>();
        optionsBuilder.UseSqlServer(ConnectionString, x => x.MigrationsAssembly(typeof(EntityFrameworkCore.SqlServer.Marker).Assembly.GetName().Name));
        using var context = new DesicContext(optionsBuilder.Options);

        Console.WriteLine($"Attempting to initialize the database");
        using var cts = new CancellationTokenSource();
        await DesicContext.InitializeAsync(context, cts.Token);
        Console.WriteLine($"Successfully initialized the database");
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
