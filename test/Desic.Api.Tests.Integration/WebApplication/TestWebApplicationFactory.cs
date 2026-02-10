using Desic.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Desic.Api.Tests.Integration.WebApplication;

public class TestWebApplicationFactory<TProgram>(string connectionString) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<DesicContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<DesicContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var inMemoryConfig = new Dictionary<string, string?>
            {
                ["DesicContext:AllowInitialization"] = "false",
                ["HttpLogging:Enabled"] = "false",
            };
            config.AddInMemoryCollection(inMemoryConfig);
        });

        builder.UseEnvironment("Development");
    }

    private static void ConfigureForSqlite(IServiceCollection services)
    {
        // create open SqliteConnection so EF won't automatically close it
        services.AddSingleton<DbConnection>(container =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddDbContext<DesicContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });
    }
}
