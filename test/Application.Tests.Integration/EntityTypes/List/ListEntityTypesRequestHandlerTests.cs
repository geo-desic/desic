using AwesomeAssertions;
using Desic.Application.EntityTypes.List;
using Desic.Testing.Integration.Db;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application.Tests.Integration.EntityTypes.List;

public class ListEntityTypesRequestHandlerTests(SeededAppDatabase testDatabase) : TestHostDependencyTests(testDatabase), IClassFixture<SeededAppDatabase>
{
    [Fact]
    public async Task ListEntityTypes_ValidRequestWithCount1_OneEntityTypeReturned()
    {
        // arrange
        var mediator = Host.Services.GetRequiredService<IMediator>();
        var request = new ListEntityTypesRequest { Count = 1, StartIndex = 0 };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count.Should().Be(1);
        result.Value.TotalCount.Should().BeGreaterThan(1);
    }
}
