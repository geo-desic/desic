using AwesomeAssertions;
using Desic.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Tests.Unit;

public class DependencyInjectionTests
{
    public class DependencyInjectionTests001 : DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_SpecifiedServiceCollection_RegistersExpectedServices()
        {
            // arrange
            var serviceCollection = NewServiceCollection();

            // act
            serviceCollection.AddInfrastructure();

            // assert
            var serviceDescriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IApplicationDbContext));
            serviceDescriptor.Should().NotBeNull();
        }
    }

    private static IServiceCollection NewServiceCollection()
    {
        return new ServiceCollection();
    }
}
