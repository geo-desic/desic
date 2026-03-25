using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using Desic.Domain.EntityTypes;
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
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListEntityTypesRequest
        {
            Pagination = new Pagination
            {
                Count = 1,
                IncludeTotalCount = true,
                StartIndex = 0,
            },
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count.Should().Be(1);
        result.Value.TotalCount.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task ListEntityTypes_ValidRequestWithNonDefaultStartIndexCountAndOrderingMethod_ExpectedResultsOrderedCorrectly()
    {
        // arrange
        var count = 3; // there needs to be at least this number of seeded entity types for this test to work correctly, see Desic.Domain.EntityTypes.SystemEntityTypes
        var startIndex = 1;
        var allOrderedByKeyDesc = SystemEntityTypes.AllAsEntities().Select(x => new Application.EntityTypes.EntityType { Name = x.Name, Key = x.Key }).OrderByDescending(x => x.Key).ToList();
        var expected = new ListEntityTypesResult
        {
            StartIndex = startIndex,
            TotalCount = allOrderedByKeyDesc.Count,
            Items = [.. allOrderedByKeyDesc.Take(startIndex..(startIndex + count))],
        };
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListEntityTypesRequest
        {
            Pagination = new Pagination
            {
                Count = count,
                IncludeTotalCount = true,
                StartIndex = startIndex,
            },
            OrderingMethod = EntityTypesOrderingMethod.KeyDesc,
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrderingFor(x => x.Items));
    }

    [Fact]
    public async Task ListEntityTypes_ValidRequestWithFilter_ExpectedResult()
    {
        // arrange
        var expectedEntityType = SystemEntityTypes.Label.ToEntity().ToModel();
        var expected = new ListEntityTypesResult
        {
            StartIndex = 0,
            TotalCount = null,
            Items = [expectedEntityType],
        };
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListEntityTypesRequest
        {
            Pagination = new Pagination
            {
                IncludeTotalCount = false,
            },
            Filter = new EntityTypesFilter { Name = expectedEntityType.Name },
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrderingFor(x => x.Items));
    }
}
