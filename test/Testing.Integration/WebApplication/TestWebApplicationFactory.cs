using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Sqlite;
using Desic.Infrastructure.Data.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace Desic.Testing.Integration.WebApplication;

public class TestWebApplicationFactory<TProgram>(string connectionString, DbProvider dbProvider) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Constants.TestEnvironmentName);

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            if (dbProvider == DbProvider.Sqlite)
            {
                services.ConfigureApplicationDbContextForSqlite(connectionString: _connectionString, setMigrationsAssembly: false, useSeeding: false);
            }
            else // SqlServer
            {
                services.ConfigureApplicationDbContextForSqlServer(connectionString: _connectionString, setMigrationsAssembly: false, useSeeding: false);
            }
        });
    }
}
