using AwesomeAssertions;
using Desic.Application.Common;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using Desic.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Desic.Application.Tests.Unit.EntityTypes.List;

public class ListEntityTypesRequestHandlerTests : InMemoryEfCoreDependencyTests<TestApplicationDbContext>
{
    private readonly ILogger<ListEntityTypesRequestHandler> _logger = NullLogger<ListEntityTypesRequestHandler>.Instance;
    private readonly Domain.EntityTypes.EntityType[] _seededEntityTypes;
    private const int DefaultCount = ListRequests.DefaultCount;
    private const EntityTypesOrderingMethod DefaultOrderingMethod = EntityTypesOrderingMethod.NameAsc;
    private const int MaximumAllowedCount = ListEntityTypesRequestHandler.MaximumAllowedCount;
    private const int TotalCount = MaximumAllowedCount + 10;

    public ListEntityTypesRequestHandlerTests() : base(o => new(o))
    {
        _seededEntityTypes = [.. GetEntityTypes()];
        DbContext.EntityTypes.AddRange(_seededEntityTypes);
        DbContext.SaveChanges();
    }

    public class ListEntityTypesRequestHandlerTests001 : ListEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(0, 0, -1, 0)]                                         // count == -1                                  =====> no items returned (negative count treated as 0)
        [InlineData(0, 0, 0, 0)]                                          // count == 0                                   =====> no items returned
        [InlineData(0, 1, 1, 0)]                                          // count == 1                                   =====> only first ordered item returned
        [InlineData(0, DefaultCount, DefaultCount, 0)]                    // count == default                             =====> default count of ordered items returned
        [InlineData(0, MaximumAllowedCount, MaximumAllowedCount, 0)]      // count == maximum                             =====> maximum count of ordered items returned
        [InlineData(0, MaximumAllowedCount, MaximumAllowedCount + 1, 0)]  // count >  maximum                             =====> maximum count of ordered items returned (count capped at maximum)
        [InlineData(0, 0, 0, -1)]                                         // count == 0       , startIndex == -1          =====> no items returned (negative startIndex treated as 0)
        [InlineData(0, 1, 1, -1)]                                         // count == 1       , startIndex == -1          =====> only the first ordered item returned (negative startIndex treated as 0)
        [InlineData(1, 1, 1, 1)]                                          // count == 1       , startIndex == 1           =====> only the second ordered item returned (first item skipped)
        [InlineData(0, 0, 1, TotalCount)]                                 // count == 1       , startIndex == total count =====> no items returned (all skipped)
        public async Task Handle_SpecifiedRequestValues_ExpectedResults(int expectedMinIndex, int expectedCount, int count, int startIndex)
        {
            // arrange
            var expectedStartIndex = startIndex < 0 ? 0 : startIndex;
            var expected = new Result<ListEntityTypesResult>(new ListEntityTypesResult
            {
                Items = [.. ExpectedItems(minIndex: expectedMinIndex, count: expectedCount)],
                StartIndex = expectedStartIndex,
                TotalCount = ListEntityTypesRequestHandler.IncludeTotalCountAllowed ? TotalCount : null,
            });
            var handler = new ListEntityTypesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListEntityTypesRequest
            {
                Pagination = new Pagination
                {
                    Count = count,
                    IncludeTotalCount = true,
                    StartIndex = startIndex,
                },
            };

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeOfType(expected.GetType());
            result.Value.Should().BeOfType(expected.Value.GetType());
            result.Value.Should().BeEquivalentTo(expected.Value);
        }
    }

    public class ListEntityTypesRequestHandlerTests002 : ListEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(EntityTypesOrderingMethod.KeyAsc)]
        [InlineData(EntityTypesOrderingMethod.KeyDesc)]
        [InlineData(EntityTypesOrderingMethod.NameAsc)]
        [InlineData(EntityTypesOrderingMethod.NameDesc)]
        public async Task Handle_SpecifiedOrderingMethod_ExpectedResultsOrderedCorrectly(EntityTypesOrderingMethod? orderingMethod)
        {
            // arrange
            var count = 10;
            var expected = new Result<ListEntityTypesResult>(new ListEntityTypesResult
            {
                // if no ordering method is specified it should default to ordering by name ascending
                Items = [.. ExpectedItems(minIndex: 0, count: count, orderingMethod: orderingMethod ?? DefaultOrderingMethod)],
                StartIndex = 0,
                TotalCount = ListEntityTypesRequestHandler.IncludeTotalCountAllowed ? TotalCount : null,
            });
            var handler = new ListEntityTypesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListEntityTypesRequest
            {
                Pagination = new Pagination
                {
                    Count = count,
                    IncludeTotalCount = true,
                    StartIndex = 0,
                },
            };
            if (orderingMethod.HasValue) request.OrderingMethod = orderingMethod.Value;

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeOfType(expected.GetType());
            result.Value.Should().BeOfType(expected.Value.GetType());
            result.Value.Should().BeEquivalentTo(expected.Value, opt => opt.WithStrictOrderingFor(x => x.Items));
        }
    }

    private static IEnumerable<Domain.EntityTypes.EntityType> GetEntityTypes()
    {
        // purposely not ordered asc by any property
        for (var i = TotalCount; i > 0; --i)
        {
            var iString = $"{i}".PadLeft(3, '0');
            char alphaCharacterCapitalized = (char)((i % 26) + 'A'); // this is to ensure that ordering by key vs name will be different
            yield return new Domain.EntityTypes.EntityType { Id = i.ToGuid(), Key = $"k{iString}", Name = $"Entity{alphaCharacterCapitalized}{iString}" };
        }
    }

    private IEnumerable<EntityType> ExpectedItems(int minIndex, int count, EntityTypesOrderingMethod orderingMethod = DefaultOrderingMethod)
    {
        // note that the OrderBy extension method used here is already covered by tests ====> see Desic.Application.Tests.Unit.EntityTypes.QueryableExtensionsTests
        return _seededEntityTypes.AsQueryable().OrderBy(orderingMethod: orderingMethod).Take(minIndex..(minIndex + count)).Select(x => new EntityType { Key = x.Key, Name = x.Name });
    }
}
