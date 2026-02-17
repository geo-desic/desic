using Desic.EntityFrameworkCore.Models;
using Desic.EntityFrameworkCore.Sqlite;
using Desic.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
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
                services.ConfigureDesicContextForSqlite(_connectionString);
            }
            else // SqlServer
            {
                services.ConfigureDesicContextForSqlServer(_connectionString);
            }
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json"), optional: true, reloadOnChange: true);
        });

        builder.UseEnvironment("Development");
    }
}
