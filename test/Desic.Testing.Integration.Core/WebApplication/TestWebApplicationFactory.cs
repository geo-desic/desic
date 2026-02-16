using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.Models.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Desic.Testing.Integration.Core.WebApplication;

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

            services.AddDbContext<DesicContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(_connectionString, x => x.MigrationsAssembly(typeof(EntityFrameworkCore.SqlServer.Marker).Assembly.GetName().Name));
                options.UseDesicContextSeeding(serviceProvider);
            });
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json"), optional: false, reloadOnChange: true);
        });

        builder.UseEnvironment("Development");
    }

    /*
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
    */
}
