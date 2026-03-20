using Desic.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Desic.Testing.Integration.Hosting;

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

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                _ = dbProvider switch
                {
                    DbProvider.Sqlite => options.UseSqlite(connectionString: _connectionString),
                    DbProvider.SqlServer => options.UseSqlServer(connectionString: _connectionString),
                    _ => throw new NotSupportedException($"Unsupported db provider: {dbProvider}"),
                };
            });
        });
    }
}
