using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Models;
using Desic.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Common.Extensions;

// note: using ef core in memory provider as it is significantly faster and these tests do not actually need a realistic database
// the application use cases that depend on these extension methods should themselves be covered under integration tests using a real database
public class QueryableExtensionsTests : InMemoryEfCoreDependencyTests<TestDbContext>
{
    private readonly Guid IdThatDoesNotExist = Guid.AllBitsSet;
    private readonly Guid IdThatExists = 2.ToGuid(); // purposely using the id in the middle of the seeded data to make sure implementation actually finds the entity with the id versus simply returning the first or last one
    private const int TotalEntityCount = 10;

    public QueryableExtensionsTests() : base(o => new(o))
    {
        DbContext.IntIdEntities.AddRange(GetTestIntIdEntities(minId: 1, maxId: TotalEntityCount));
        DbContext.TestEntities.AddRange(GetTestEntities(minIndex: 1, maxIndex: TotalEntityCount));
        DbContext.SaveChanges();
    }

    public class QueryableExtensionsTests001 : QueryableExtensionsTests
    {
        [Fact]
        public async Task GetEntityByIdAsync_EntityWithSpecifiedIdExists_ReturnsExpectedEntity()
        {
            // arrange
            var expected = new TestEntity { Id = IdThatExists };

            // act
            var result = await QueryableExtensions.GetEntityByIdAsync(source: DbContext.TestEntities, id: IdThatExists, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests002 : QueryableExtensionsTests
    {
        [Fact]
        public async Task GetEntityByIdAsync_EntityWithSpecifiedIdDoesNotExist_ReturnsNull()
        {
            // act
            var result = await QueryableExtensions.GetEntityByIdAsync(source: DbContext.TestEntities, id: IdThatDoesNotExist, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
        }
    }

    public class QueryableExtensionsTests003 : QueryableExtensionsTests
    {
        [Fact]
        public async Task GetEntityByIdQuery_EntityWithSpecifiedIdExists_ReturnsExpectedQuery()
        {
            // arrange
            var expected = new TestEntity { Id = IdThatExists };

            // act
            var query = QueryableExtensions.GetEntityByIdQuery(source: DbContext.TestEntities, id: IdThatExists);
            var result = await query.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests004 : QueryableExtensionsTests
    {
        [Fact]
        public async Task GetEntityByIdQuery_EntityWithSpecifiedIdDoesNotExist_ReturnsExpectedQuery()
        {
            // act
            var query = QueryableExtensions.GetEntityByIdQuery(source: DbContext.TestEntities, id: IdThatDoesNotExist);
            var result = await query.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
        }
    }

    // at quick glance tests may look like they are duplicated twice, but each set are acting on different extension methods
    public class QueryableExtensionsTests005 : QueryableExtensionsTests
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
            var expected = new ListResult<IntIdEntity> { Items = GetTestIntIdEntities(minId: expectedMinId, maxId: expectedMaxId), StartIndex = startIndex };
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

    public class QueryableExtensionsTests006 : QueryableExtensionsTests
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
            var expected = new ListIntIdEntitiesResult { Items = GetTestIntIdEntities(minId: expectedMinId, maxId: expectedMaxId), StartIndex = startIndex };
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

    public class QueryableExtensionsTests007 : QueryableExtensionsTests
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

    public class QueryableExtensionsTests008 : QueryableExtensionsTests
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

    public class QueryableExtensionsTests009 : QueryableExtensionsTests
    {
        [Fact]
        public async Task ToListResultAsync_Take2WithOrderByDescending_Last2ItemsInReverseOrderReturned()
        {
            // arrange
            var items = GetTestIntIdEntities(minId: TotalEntityCount - 1, maxId: TotalEntityCount);
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

    public class QueryableExtensionsTests010 : QueryableExtensionsTests
    {
        [Fact]
        public async Task ToListResultAsyncTR_Take2WithOrderByDescending_Last2ItemsInReverseOrderReturned()
        {
            // arrange
            var items = GetTestIntIdEntities(minId: TotalEntityCount - 1, maxId: TotalEntityCount);
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

    private static List<TestEntity> GetTestEntities(int minIndex, int maxIndex)
    {
        var result = new List<TestEntity>();
        for (var i = minIndex; i <= maxIndex; ++i)
        {
            result.Add(new TestEntity { Id = i.ToGuid() });
        }
        return result;
    }

    private static List<IntIdEntity> GetTestIntIdEntities(int minId, int maxId)
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
