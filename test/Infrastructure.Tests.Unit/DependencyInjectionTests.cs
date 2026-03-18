using AwesomeAssertions;
using Desic.Application.Common.Interfaces;
using Desic.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Desic.Infrastructure.Tests.Unit;

public class DependencyInjectionTests
{
    public class DependencyInjectionTests001 : DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_ToServiceCollection_RegistersExpectedServices()
        {
            // arrange
            var serviceCollection = NewServiceCollection();

            // act
            serviceCollection.AddInfrastructure();

            // assert
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IApplicationDbContext)).Should().NotBeNull();
            // at least one request handler is registered (MediatR)
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IRequestHandler<SeedApplicationDatabaseRequest>)).Should().NotBeNull();
            // at least one IOptions is registered
            var serviceProvider = serviceCollection.BuildServiceProvider(); // can't validate IOptions<T> until service provider is built
            serviceProvider.GetService<IOptions<ApplicationDatabaseSeedingOptions>>().Should().NotBeNull();
        }
    }

    private static IServiceCollection NewServiceCollection()
    {
        var result = new ServiceCollection();
        result.AddSingleton<IConfiguration>(NewConfiguration());
        return result;
    }

    private static IConfigurationRoot NewConfiguration()
    {
        return new ConfigurationBuilder().AddInMemoryCollection([]).Build();
    }
}
