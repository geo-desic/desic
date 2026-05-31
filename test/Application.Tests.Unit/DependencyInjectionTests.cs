using AwesomeAssertions;
using Desic.Application.Common;
using Desic.Application.EntityTypes.List;
using Desic.Application.Users.Create;
using DispatchR.Abstractions.Send;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application.Tests.Unit;

public class DependencyInjectionTests
{
    public class DependencyInjectionTests001 : DependencyInjectionTests
    {
        [Fact]
        public void AddApplication_ToServiceCollection_RegistersExpectedServices()
        {
            // arrange
            var serviceCollection = NewServiceCollection();

            // act
            serviceCollection.AddApplication();

            // assert
            // at least one validator is registered (FluentAssertions)
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IValidator<CreateUser>)).Should().NotBeNull();
            // at least one request handler is registered
            serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IRequestHandler<ListEntityTypesRequest, Task<Result<ListEntityTypesResult>>>)).Should().NotBeNull();
        }
    }

    private static IServiceCollection NewServiceCollection()
    {
        return new ServiceCollection();
    }
}
