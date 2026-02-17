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
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

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

            if (TestConfiguration.Options?.DbProvider == "Sqlite")
            {
                ConfigureForSqlite(services);
            }
            else // SqlServer
            {
                ConfigureForSqlServer(services);
            }
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json"), optional: false, reloadOnChange: true);
        });

        builder.UseEnvironment("Development");
    }

    private void ConfigureForSqlite(IServiceCollection services)
    {
        services.AddDbContext<DesicContext>((serviceProvider, options) =>
        {
            options.UseSqlite(_connectionString, x => x.MigrationsAssembly(typeof(EntityFrameworkCore.SqlServer.IMarker).Assembly.GetName().Name));
            options.UseDesicContextSeeding(serviceProvider);
        });
    }

    private void ConfigureForSqlServer(IServiceCollection services)
    {
        services.AddDbContext<DesicContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(_connectionString, x => x.MigrationsAssembly(typeof(EntityFrameworkCore.SqlServer.IMarker).Assembly.GetName().Name));
            options.UseDesicContextSeeding(serviceProvider);
        });
    }
}
