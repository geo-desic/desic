using AwesomeAssertions;
using Desic.Domain.EntityTypes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Domain.Tests.Unit;

public class DependencyInjectionTests
{
    public class DependencyInjectionTests001 : DependencyInjectionTests
    {
        [Fact]
        public void AddDomain_ToServiceCollection_RegistersExpectedServices()
        {
            // arrange
            var serviceCollection = NewServiceCollection();

            // act
            serviceCollection.AddDomain();

            // assert
            // currently no use of mediatR in the assembly
            // at least one validator is registered (FluentAssertions)
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IValidator<IReadOnlyEntityType>)).Should().NotBeNull();
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IValidator<SystemEntityType>)).Should().NotBeNull();
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IValidator<EntityType>)).Should().NotBeNull();
        }
    }

    private static IServiceCollection NewServiceCollection()
    {
        return new ServiceCollection();
    }
}
