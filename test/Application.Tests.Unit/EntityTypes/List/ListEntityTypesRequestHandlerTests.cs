
using AwesomeAssertions;
using Desic.Application.Common;
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
    private const int TotalCount = 150; // greater than ListEntityTypesRequestHandler.MaximumAllowedCount

    public ListEntityTypesRequestHandlerTests() : base(o => new(o))
    {
        _seededEntityTypes = GetEntityTypes().ToArray();
        DbContext.EntityTypes.AddRange(_seededEntityTypes);
        DbContext.SaveChanges();
    }

    public class ListEntityTypesRequestHandlerTests001 : ListEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(0, ListEntityTypesRequestHandler.MaximumAllowedCount, null, null)] // count == null, startIndex == null =====> maximum allowed ordered items returned
        [InlineData(1, ListEntityTypesRequestHandler.MaximumAllowedCount, null, 1)]    // count == null, startIndex == 1    =====> maximum allowed ordered items returned except the first
        [InlineData(0, 1, 1, null)]                                                    // count == 1   , startIndex == null =====> only first ordered item returned
        [InlineData(0, 1, 1, -1)]                                                      // count == 1   , startIndex == -1   =====> only first ordered item returned (negative startIndex treated as 0)
        [InlineData(1, 1, 1, 1)]                                                       // count == 1   , startIndex == 1    =====> only the second ordered item returned
        public async Task Handle_SpecifiedRequestValues_ExpectedResults(int expectedMinIndex, int expectedCount, int? count, int? startIndex)
        {
            // arrange
            var expectedStartIndex = startIndex ?? 0;
            if (expectedStartIndex < 0) expectedStartIndex = 0;
            var expected = new Result<ListEntityTypesResult>(new ListEntityTypesResult
            {
                Items = [.. ExpectedItems(minIndex: expectedMinIndex, count: expectedCount)],
                StartIndex = expectedStartIndex,
                TotalCount = ListEntityTypesRequestHandler.IncludeTotalCount ? TotalCount : null,
            });
            var handler = new ListEntityTypesRequestHandler(logger: _logger, dbContext: DbContext);
            var request = new ListEntityTypesRequest { Count = count, StartIndex = startIndex };

            // act
            var result = await handler.Handle(request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeOfType(expected.GetType());
            result.Value.Should().BeOfType(expected.Value.GetType());
            result.Value.Should().BeEquivalentTo(expected.Value);
        }
    }

    private static IEnumerable<Domain.EntityTypes.EntityType> GetEntityTypes()
    {
        // purposely not ordered by name
        for (var i = TotalCount; i > 0; --i)
        {
            var iString = $"{i}".PadLeft(3, '0');
            yield return new Domain.EntityTypes.EntityType { Id = i.ToGuid(), Key = $"k{iString}", Name = $"Entity{iString}" };
        }
    }

    private IEnumerable<EntityType> ExpectedItems(int minIndex, int count)
    {
        return _seededEntityTypes.OrderBy(x => x.Name).Skip(minIndex).Take(count).Select(x => new EntityType { Key = x.Key, Name = x.Name });
    }
}
