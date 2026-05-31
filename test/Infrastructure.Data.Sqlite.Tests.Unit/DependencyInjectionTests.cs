using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.Sqlite.Tests.Unit;

public class DependencyInjectionTests
{
    public const string ConnectionString = "Filename=:memory:";

    public class DependencyInjectionTests001 : DependencyInjectionTests
    {
        [Theory]
        [InlineData(ConnectionString, null, null)]
        [InlineData(null, ConnectionString, null)]
        [InlineData(null, null, ConnectionString)]
        public void AddSqliteInfrastructure_ToServiceCollectionWihtSpecifiedConnectionString_RegistersExpectedServices(string? explicitConnectionString, string? configConnection, string? configConnectionStringsSqlite)
        {
            // arrange
            var configuration = NewConfiguration(connection: configConnection, connectionStringsSqlite: configConnectionStringsSqlite);
            var serviceCollection = NewServiceCollection(configuration: configuration);

            // act
            serviceCollection.AddSqliteInfrastructure(config: configuration, connectionString: explicitConnectionString);

            // assert
            // currently no use of mediator in the assembly
            // assert ApplicationDbContext is registered correctly
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.GetConnectionString().Should().Be(ConnectionString);
        }
    }

    public class DependencyInjectionTests002 : DependencyInjectionTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void AddSqliteInfrastructure_ToServiceCollectionWihtSpecifiedConnectionString_RegistersExpectedServices(bool? migrationsEnabled)
        {
            // arrange
            var expectedMigrationsEnabled = migrationsEnabled ?? true;
            // if migrations assembly isn't explicitly set, ef core assumes it is the same assembly as the target DbContext
            var expectedMigrationsAssembly = expectedMigrationsEnabled ? typeof(IAssemblyReference).Assembly : typeof(ApplicationDbContext).Assembly;
            var configuration = NewConfiguration(migrationsEnabled: migrationsEnabled);
            var serviceCollection = NewServiceCollection(configuration: configuration);

            // act
            serviceCollection.AddSqliteInfrastructure(config: configuration, connectionString: ConnectionString);

            // assert
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.GetService<IMigrationsAssembly>()?.Assembly?.FullName.Should().Be(expectedMigrationsAssembly?.FullName);
        }
    }

    private static IServiceCollection NewServiceCollection(IConfiguration? configuration = null)
    {
        var result = new ServiceCollection();
        if (configuration != null) result.AddSingleton(configuration);
        return result;
    }

    private static IConfigurationRoot NewConfiguration(string? connection = null, string? connectionStringsSqlite = null, bool? migrationsEnabled = null, bool? seedingEnabled = null)
    {
        var dictionary = new Dictionary<string, string?>();
        if (connection != null) dictionary.Add("connection", connection);
        if (connectionStringsSqlite != null) dictionary.Add("ConnectionStrings:Sqlite", connectionStringsSqlite);
        if (migrationsEnabled.HasValue) dictionary.Add(ApplicationDatabaseConfigKeys.MigrationsEnabled, $"{migrationsEnabled}");
        if (seedingEnabled.HasValue) dictionary.Add(ApplicationDatabaseConfigKeys.SeedingEnabled, $"{seedingEnabled}");
        return new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
    }
}
