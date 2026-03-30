using AwesomeAssertions;
using Desic.Application.Common;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using Desic.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Desic.Application.Tests.Unit.Iso3166Countries.List;

public class ListIso3166CountriesRequestHandlerTests : InMemoryEfCoreDependencyTests<TestApplicationDbContext>
{
    private readonly ILogger<ListIso3166CountriesRequestHandler> _logger = NullLogger<ListIso3166CountriesRequestHandler>.Instance;
    private readonly Domain.Iso3166Countries.Iso3166Country[] _seededEntities;
    private const int MaximumAllowedCount = ListIso3166CountriesRequestHandler.MaximumAllowedCount;
    private const int TotalCount = MaximumAllowedCount + 10;

    public ListIso3166CountriesRequestHandlerTests() : base(o => new(o))
    {
        _seededEntities = [.. GetEntities()];
        DbContext.Iso3166Countries.AddRange(_seededEntities);
        DbContext.SaveChanges();
    }

    public class ListIso3166CountriesRequestHandlerTests001 : ListIso3166CountriesRequestHandlerTests
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
            var expected = new Result<ListIso3166CountriesResult>(new ListIso3166CountriesResult
            {
                Items = [.. ExpectedItems(minIndex: expectedMinIndex, count: expectedCount)],
                StartIndex = expectedStartIndex,
                TotalCount = ListIso3166CountriesRequestHandler.IncludeTotalCountAllowed ? TotalCount : null,
            });
            var handler = new ListIso3166CountriesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListIso3166CountriesRequest
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

    public class ListIso3166CountriesRequestHandlerTests002 : ListIso3166CountriesRequestHandlerTests
    {
        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Alpha2, true, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Alpha2, false, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Alpha3, true, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Alpha3, false, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Id, true, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Id, false, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.IsoId, true, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.IsoId, false, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Name, true, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Name, false, null, null)]
        [InlineData(Iso3166CountriesOrderingProperty.Alpha2, true, Iso3166CountriesOrderingProperty.Id, false)]
        public async Task Handle_SpecifiedOrderingMethod_ExpectedResultsOrderedCorrectly(Iso3166CountriesOrderingProperty? property1, bool? ascending1, Iso3166CountriesOrderingProperty? property2, bool? ascending2)
        {
            // arrange
            var count = 10;
            OrderingMethod<Iso3166CountriesOrderingProperty>? orderingMethod = null;
            if (property1 != null && ascending1.HasValue)
            {
                orderingMethod = new OrderingMethod<Iso3166CountriesOrderingProperty>
                {
                    OrderBy =
                    [
                        new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = ascending1.Value, Property = property1.Value },
                    ],
                };
                if (property2 != null && ascending2.HasValue)
                {
                    orderingMethod.OrderBy.Add(new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = ascending2.Value, Property = property2.Value });
                }
            }
            var expected = new Result<ListIso3166CountriesResult>(new ListIso3166CountriesResult
            {
                // if no ordering method is specified it should use the default
                Items = [.. ExpectedItems(minIndex: 0, count: count, orderingMethod: orderingMethod ?? default)],
                StartIndex = 0,
                TotalCount = null,
            });
            var handler = new ListIso3166CountriesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListIso3166CountriesRequest
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

    public class ListIso3166CountriesRequestHandlerTests003 : ListIso3166CountriesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_WithFilterMatchingOneItem_ItemReturnedInResult()
        {
            // arrange
            var expectedIsoId = TotalCount - 5; // should be 5th seeded in constructor
            var handler = new ListIso3166CountriesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListIso3166CountriesRequest
            {
                Pagination = new Pagination
                {
                    IncludeTotalCount = false,
                },
                Filter = new()
                {
                    IsoId = expectedIsoId,
                }
            };

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Count.Should().Be(1);
            result.Value.Items.ElementAt(0).IsoId.Should().Be(expectedIsoId);
        }
    }

    private static IEnumerable<Domain.Iso3166Countries.Iso3166Country> GetEntities()
    {
        // purposely in no particular order and ordering by any property should result in different orderings
        for (var i = 0; i < TotalCount; ++i)
        {
            var iString = $"{i}".PadLeft(4, '0');
            char alpha2Character1 = (char)((i % 26) + 'a');
            char alpha2Character2 = (char)(((i / 26) % 26) + 'a');
            char alpha3Character1 = (char)('z' - (i % 26));
            char alpha3Character2 = (char)('z' - ((i / 26) % 26));
            char alpha3Character3 = (char)('z' - ((i / 26 / 26) % 26));
            char nameCharacter = (char)(((i + 1) % 26) + 'A');
            yield return new Domain.Iso3166Countries.Iso3166Country
            {
                Alpha2 = $"{alpha2Character1}{alpha2Character2}",
                Alpha3 = $"{alpha3Character1}{alpha3Character2}{alpha3Character3}",
                Id = i == 0 ? (TotalCount + 1).ToGuid() : i.ToGuid(),
                IsoId = TotalCount - i,
                Name = $"Name{nameCharacter}{iString}",
            };
        }
    }

    private IEnumerable<Iso3166CountryView> ExpectedItems(int minIndex, int count, OrderingMethod<Iso3166CountriesOrderingProperty>? orderingMethod = null)
    {
        orderingMethod ??= OrderingMethod<Iso3166CountriesOrderingProperty>.Default;
        // note that the OrderBy and SelectToModel extension methods used here are already covered by tests of their own
        return _seededEntities.AsQueryable().OrderBy(orderingMethod: orderingMethod).Take(minIndex..(minIndex + count)).SelectToView();
    }
}
