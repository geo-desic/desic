using AwesomeAssertions;
using Desic.Application.Common.Helpers;
using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Common.Helpers;

// note: using ef core in memory provider as it is significantly faster and do not actually need a realistic test database for these tests
// the application use cases that depend on these helpers should be covered under integration tests which use a real database
public class QueryableHelpersTests : IDisposable, IAsyncDisposable
{
    private readonly TestDbContext _context;
    private bool _disposed = false;
    private readonly DbContextOptions<TestDbContext> _options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString()) // unique name ensures isolation between tests
        .Options;
    private const int TotalEntityCount = 10;

    public QueryableHelpersTests()
    {
        _context = new(_options);
        _context.Database.EnsureCreated();
        _context.TestEntities.AddRange(GetTestEntities(minId: 1, maxId: TotalEntityCount));
        _context.SaveChanges();
    }

    public class PaginatedListTests001 : QueryableHelpersTests
    {
        [Theory]
        [InlineData(1, 1, 0, 1)] // takeCount is 1 => only the first item returned
        [InlineData(1, TotalEntityCount - 1, 0, TotalEntityCount - 1)] // takeCount is one less than total number of items => all items except the last returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount)] // takeCount is the total number of items => all items returned
        [InlineData(1, TotalEntityCount, 0, TotalEntityCount + 1)] // takeCount is one greater than total number of items => all items returned
        [InlineData(2, TotalEntityCount, 1, TotalEntityCount)] // startIndex is 1 => all items except the first returned
        [InlineData(TotalEntityCount, TotalEntityCount, TotalEntityCount - 1, TotalEntityCount)] // startIndex is one less than the total number of items => only the last item returned
        [InlineData(TotalEntityCount + 1, TotalEntityCount, TotalEntityCount, TotalEntityCount)] // startIndex is the total number of items => no items returned
        [InlineData(2, 2, 1, 1)] // startIndex is 1 and takeCount is 1 => only the second item returned
        [InlineData(3, 3, 2, 1)] // startIndex is 2 and takeCount is 1 => only the third item returned
        public async Task ToListResultAsync_SpecifiedArguments_ResultContainsExpectedItems(int expectedMinId, int expectedMaxId, int startIndex, int takeCount)
        {
            // arrange
            var expected = new ListResult<TestEntity>(items: GetTestEntities(minId: expectedMinId, maxId: expectedMaxId), startIndex: startIndex);
            var source = _context.TestEntities.AsQueryable();

            // act
            var result = await QueryableHelpers.ToListResultAsync(source: source, startIndex: startIndex, takeCount: takeCount, includeTotalCount: false, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    public class PaginatedListTests002 : QueryableHelpersTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ToListResultAsync_SpecifiedIncludeTotalCount_TotalCountOnlyIncludedIfSpecified(bool includeTotalCount)
        {
            // arrange
            var source = _context.TestEntities.AsQueryable();

            // act
            var result = await QueryableHelpers.ToListResultAsync(source: source, startIndex: 0, takeCount: 1, includeTotalCount: includeTotalCount, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.TotalCount.Should().Be(includeTotalCount ? TotalEntityCount : null);
        }
    }

    public class PaginatedListTests003 : QueryableHelpersTests
    {
        [Fact]
        public async Task ToListResultAsync_Take2WithOrderByDescending_Last2ItemsInReverseOrderReturned()
        {
            // arrange
            var items = GetTestEntities(minId: TotalEntityCount - 1, maxId: TotalEntityCount);
            items.Reverse();
            var expected = new ListResult<TestEntity>(items: items, startIndex: 0);
            var source = _context.TestEntities.AsQueryable().OrderByDescending(x => x.Id);

            // act
            var result = await QueryableHelpers.ToListResultAsync(source: source, startIndex: 0, takeCount: 2, includeTotalCount: false, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrderingFor(x => x.Items));
        }
    }

    private static List<TestEntity> GetTestEntities(int minId, int maxId)
    {
        var result = new List<TestEntity>();
        for (var i = minId; i <= maxId; ++i)
        {
            result.Add(new TestEntity { Id = i });
        }
        return result;
    }

    private class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    private class TestEntity
    {
        public int Id { get; set; }
    }

    #region disposable
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed) return;
        if (_context is IAsyncDisposable asyncDisposableResource)
        {
            await asyncDisposableResource.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _context?.Dispose();
        }
        _disposed = true;
    }
    #endregion
}
