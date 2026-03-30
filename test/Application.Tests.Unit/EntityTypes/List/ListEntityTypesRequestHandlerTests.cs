using AwesomeAssertions;
using Desic.Application.Common;
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
    private readonly Domain.EntityTypes.EntityType[] _seededEntities;
    private const int MaximumAllowedCount = ListEntityTypesRequestHandler.MaximumAllowedCount;
    private const int TotalCount = MaximumAllowedCount + 10;

    public ListEntityTypesRequestHandlerTests() : base(o => new(o))
    {
        _seededEntities = [.. GetEntities()];
        DbContext.EntityTypes.AddRange(_seededEntities);
        DbContext.SaveChanges();
    }

    public class ListEntityTypesRequestHandlerTests001 : ListEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(0, 0, -1, 0)]                                         // count == -1                                  =====> no items returned (negative count treated as 0)
        [InlineData(0, 0, 0, 0)]                                          // count == 0                                   =====> no items returned
        [InlineData(0, 1, 1, 0)]                                          // count == 1                                   =====> only first ordered item returned
        [InlineData(0, MaximumAllowedCount, MaximumAllowedCount, 0)]      // count == maximum                             =====> maximum count of ordered items returned
        [InlineData(0, MaximumAllowedCount, MaximumAllowedCount + 1, 0)]  // count >  maximum                             =====> maximum count of ordered items returned (count capped at maximum)
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
        [InlineData(null, null, null, null)]
        [InlineData(EntityTypesOrderingProperty.Key, true, null, null)]
        [InlineData(EntityTypesOrderingProperty.Name, true, null, null)]
        [InlineData(EntityTypesOrderingProperty.Key, false, EntityTypesOrderingProperty.Name, true)]
        [InlineData(EntityTypesOrderingProperty.Name, false, EntityTypesOrderingProperty.Key, true)]
        public async Task Handle_SpecifiedOrderingMethod_ExpectedResultsOrderedCorrectly(EntityTypesOrderingProperty? property1, bool? ascending1, EntityTypesOrderingProperty? property2, bool? ascending2)
        {
            // arrange
            var count = 10;
            OrderingMethod<EntityTypesOrderingProperty>? orderingMethod = null;
            if (property1 != null && ascending1.HasValue)
            {
                orderingMethod = new OrderingMethod<EntityTypesOrderingProperty>
                {
                    OrderBy =
                    [
                        new OrderBy<EntityTypesOrderingProperty> { Ascending = ascending1.Value, Property = property1.Value },
                    ],
                };
                if (property2 != null && ascending2.HasValue)
                {
                    orderingMethod.OrderBy.Add(new OrderBy<EntityTypesOrderingProperty> { Ascending = ascending2.Value, Property = property2.Value });
                }
            }
            var expected = new Result<ListEntityTypesResult>(new ListEntityTypesResult
            {
                // if no ordering method is specified it should use the default
                Items = [.. ExpectedItems(minIndex: 0, count: count, orderingMethod: orderingMethod)],
                StartIndex = 0,
                TotalCount = null,
            });
            var handler = new ListEntityTypesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListEntityTypesRequest
            {
                Pagination = new Pagination
                {
                    Count = count,
                    IncludeTotalCount = false,
                    StartIndex = 0,
                },
            };
            if (orderingMethod != null) request.OrderingMethod = orderingMethod;

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeOfType(expected.GetType());
            result.Value.Should().BeOfType(expected.Value.GetType());
            result.Value.Should().BeEquivalentTo(expected.Value, opt => opt.WithStrictOrderingFor(x => x.Items));
        }
    }

    public class ListEntityTypesRequestHandlerTests003 : ListEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_WithFilterMatchingOneItem_ItemReturnedInResult()
        {
            // arrange
            var expectedKey = "e004"; // should be 5th item seeded in constructor
            var handler = new ListEntityTypesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListEntityTypesRequest
            {
                Pagination = new Pagination
                {
                    IncludeTotalCount = false,
                },
                Filter = new()
                {
                    Key = expectedKey,
                }
            };

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Count.Should().Be(1);
            result.Value.Items.ElementAt(0).Key.Should().Be(expectedKey);
        }
    }

    private static IEnumerable<Domain.EntityTypes.EntityType> GetEntities()
    {
        // purposely in no particular order and ordering by any property should result in different orderings
        for (var i = 0; i < TotalCount; ++i)
        {
            var iString = $"{i}".PadLeft(3, '0');
            char keyCharacter = (char)((i % 26) + 'a');
            char nameCharacter = (char)('Z' - (i % 26));
            yield return new Domain.EntityTypes.EntityType
            {
                Id = i == 0 ? (TotalCount + 1).ToGuid() : i.ToGuid(),
                Key = $"{keyCharacter}{iString}",
                Name = $"Entity{nameCharacter}{iString}"
            };
        }
    }

    private IEnumerable<EntityType> ExpectedItems(int minIndex, int count, OrderingMethod<EntityTypesOrderingProperty>? orderingMethod = null)
    {
        orderingMethod ??= OrderingMethod<EntityTypesOrderingProperty>.Default;
        // note that the OrderBy and SelectToModel extension methods used here are already covered by tests of their own
        return _seededEntities.AsQueryable().OrderBy(orderingMethod: orderingMethod).Take(minIndex..(minIndex + count)).SelectToModel();
    }
}