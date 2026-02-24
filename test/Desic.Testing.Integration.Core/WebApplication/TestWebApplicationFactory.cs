using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Sqlite;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace Desic.Testing.Integration.Core.WebApplication;

public class TestWebApplicationFactory<TProgram>(string connectionString) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

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
    }
}
