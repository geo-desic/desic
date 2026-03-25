using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Models;

namespace Desic.Application.Tests.Unit.Common.Extensions;

// note: using ef core in memory provider as it is significantly faster and these tests do not actually need a realistic database
// the application use cases that depend on these extension methods should themselves be covered under integration tests using a real database
public class QueryableExtensionsTests : InMemoryEfCoreDependencyTests<TestDbContext>
{
    private const int TotalEntityCount = 10;

    public QueryableExtensionsTests() : base(o => new(o))
    {
        DbContext.IntIdEntities.AddRange(GetTestEntities(minId: 1, maxId: TotalEntityCount));
        DbContext.SaveChanges();
    }

    // at quick glance tests may look like they are duplicated twice, but each set are acting on different extension methods
    public class QueryableExtensionsTests001 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(1, 1, 0, 1)] // count is 1 => only the first item returned
        [InlineData(1, TotalEntityCount - 1, 0, TotalEntityCount - 1)] // count is one less than total number of items => all items except the last returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount)] // count is the total number of items => all items returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount + 1)] // count is one greater than total number of items => all items returned
        [InlineData(2, TotalEntityCount, 1, TotalEntityCount)] // startIndex is 1 => all items except the first returned
        [InlineData(TotalEntityCount, TotalEntityCount, TotalEntityCount - 1, TotalEntityCount)] // startIndex is one less than the total number of items => only the last item returned
        [InlineData(TotalEntityCount + 1, TotalEntityCount, TotalEntityCount, TotalEntityCount)] // startIndex is the total number of items => no items returned
        [InlineData(2, 2, 1, 1)] // startIndex is 1 and count is 1 => only the second item returned
        [InlineData(3, 3, 2, 1)] // startIndex is 2 and count is 1 => only the third item returned
        public async Task ToListResultAsync_SpecifiedArguments_ResultContainsExpectedItems(int expectedMinId, int expectedMaxId, int startIndex, int count)
        {
            // arrange
            var expected = new ListResult<IntIdEntity> { Items = GetTestEntities(minId: expectedMinId, maxId: expectedMaxId), StartIndex = startIndex };
            var source = DbContext.IntIdEntities.AsQueryable();
            var pagination = new Pagination
            {
                Count = count,
                IncludeTotalCount = false,
                StartIndex = startIndex,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    public class QueryableExtensionsTests002 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(1, 1, 0, 1)] // count is 1 => only the first item returned
        [InlineData(1, TotalEntityCount - 1, 0, TotalEntityCount - 1)] // count is one less than total number of items => all items except the last returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount)] // count is the total number of items => all items returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount + 1)] // count is one greater than total number of items => all items returned
        [InlineData(2, TotalEntityCount, 1, TotalEntityCount)] // startIndex is 1 => all items except the first returned
        [InlineData(TotalEntityCount, TotalEntityCount, TotalEntityCount - 1, TotalEntityCount)] // startIndex is one less than the total number of items => only the last item returned
        [InlineData(TotalEntityCount + 1, TotalEntityCount, TotalEntityCount, TotalEntityCount)] // startIndex is the total number of items => no items returned
        [InlineData(2, 2, 1, 1)] // startIndex is 1 and count is 1 => only the second item returned
        [InlineData(3, 3, 2, 1)] // startIndex is 2 and count is 1 => only the third item returned
        public async Task ToListResultAsyncTR_SpecifiedArguments_ResultContainsExpectedItems(int expectedMinId, int expectedMaxId, int startIndex, int count)
        {
            // arrange
            var expected = new ListIntIdEntitiesResult { Items = GetTestEntities(minId: expectedMinId, maxId: expectedMaxId), StartIndex = startIndex };
            var source = DbContext.IntIdEntities.AsQueryable();
            var pagination = new Pagination
            {
                Count = count,
                IncludeTotalCount = false,
                StartIndex = startIndex,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync<IntIdEntity, ListIntIdEntitiesResult>(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeOfType(expected.GetType());
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    public class QueryableExtensionsTests003 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ToListResultAsync_SpecifiedIncludeTotalCount_TotalCountOnlyIncludedIfSpecified(bool includeTotalCount)
        {
            // arrange
            var source = DbContext.IntIdEntities.AsQueryable();
            var pagination = new Pagination
            {
                Count = 1,
                IncludeTotalCount = includeTotalCount,
                StartIndex = 0,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.TotalCount.Should().Be(includeTotalCount ? TotalEntityCount : null);
        }
    }

    public class QueryableExtensionsTests004 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ToListResultAsyncTR_SpecifiedIncludeTotalCount_TotalCountOnlyIncludedIfSpecified(bool includeTotalCount)
        {
            // arrange
            var source = DbContext.IntIdEntities.AsQueryable();
            var pagination = new Pagination
            {
                Count = 1,
                IncludeTotalCount = includeTotalCount,
                StartIndex = 0,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync<IntIdEntity, ListIntIdEntitiesResult>(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.TotalCount.Should().Be(includeTotalCount ? TotalEntityCount : null);
        }
    }

    public class QueryableExtensionsTests005 : QueryableExtensionsTests
    {
        [Fact]
        public async Task ToListResultAsync_Take2WithOrderByDescending_Last2ItemsInReverseOrderReturned()
        {
            // arrange
            var items = GetTestEntities(minId: TotalEntityCount - 1, maxId: TotalEntityCount);
            items.Reverse();
            var expected = new ListResult<IntIdEntity> { Items = items, StartIndex = 0 };
            var source = DbContext.IntIdEntities.AsQueryable().OrderByDescending(x => x.Id);
            var pagination = new Pagination
            {
                Count = 2,
                IncludeTotalCount = false,
                StartIndex = 0,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    public class QueryableExtensionsTests006 : QueryableExtensionsTests
    {
        [Fact]
        public async Task ToListResultAsyncTR_Take2WithOrderByDescending_Last2ItemsInReverseOrderReturned()
        {
            // arrange
            var items = GetTestEntities(minId: TotalEntityCount - 1, maxId: TotalEntityCount);
            items.Reverse();
            var expected = new ListIntIdEntitiesResult { Items = items, StartIndex = 0 };
            var source = DbContext.IntIdEntities.AsQueryable().OrderByDescending(x => x.Id);
            var pagination = new Pagination
            {
                Count = 2,
                IncludeTotalCount = false,
                StartIndex = 0,
            };

            // act
            var result = await QueryableExtensions.ToListResultAsync<IntIdEntity, ListIntIdEntitiesResult>(source: source, pagination: pagination, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    private static List<IntIdEntity> GetTestEntities(int minId, int maxId)
    {
        var result = new List<IntIdEntity>();
        for (var i = minId; i <= maxId; ++i)
        {
            result.Add(new IntIdEntity { Id = i });
        }
        return result;
    }

    private class ListIntIdEntitiesResult : ListResult<IntIdEntity> { }
}
