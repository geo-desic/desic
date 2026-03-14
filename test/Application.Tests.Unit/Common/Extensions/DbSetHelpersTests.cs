using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class DbSetHelpersTests : IDisposable, IAsyncDisposable
{
    private readonly TestDbContext _context;
    private bool _disposed = false;
    private readonly DbContextOptions<TestDbContext> _options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString()) // unique name ensures isolation between tests
        .Options;
    private const int TotalEntityCount = 3;
    private readonly Guid IdThatExists = 2.ToGuid(); // purposely using the id in the middle of the seeded data to make sure implementation actually finds the entity with the id versus simply returning the first or last one
    private readonly Guid IdThatDoesNotExist = Guid.AllBitsSet;

    public DbSetHelpersTests()
    {
        _context = new(_options);
        _context.Database.EnsureCreated();
        _context.TestEntities.AddRange(GetTestEntities(minIndex: 1, maxIndex: TotalEntityCount));
        _context.SaveChanges();
    }

    public class DbSetHelpersTests001 : DbSetHelpersTests
    {
        [Fact]
        public async Task GetEntityByIdAsync_EntityWithSpecifiedIdExists_ReturnsExpectedEntity()
        {
            // arrange
            var expected = new TestEntity { Id = IdThatExists };

            // act
            var result = await DbSetHelpers.GetEntityByIdAsync(dbSet: _context.TestEntities, id: IdThatExists, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class DbSetHelpersTests002 : DbSetHelpersTests
    {
        [Fact]
        public async Task GetEntityByIdAsync_EntityWithSpecifiedIdDoesNotExist_ReturnsNull()
        {
            // act
            var result = await DbSetHelpers.GetEntityByIdAsync(dbSet: _context.TestEntities, id: IdThatDoesNotExist, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
        }
    }

    public class DbSetHelpersTests003 : DbSetHelpersTests
    {
        [Fact]
        public async Task GetEntityByIdQuery_EntityWithSpecifiedIdExists_ReturnsExpectedQuery()
        {
            // arrange
            var expected = new TestEntity { Id = IdThatExists };

            // act
            var query = DbSetHelpers.GetEntityByIdQuery(dbSet: _context.TestEntities, id: IdThatExists, cancellationToken: TestContext.Current.CancellationToken);
            var result = await query.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class DbSetHelpersTests004 : DbSetHelpersTests
    {
        [Fact]
        public async Task GetEntityByIdQuery_EntityWithSpecifiedIdDoesNotExist_ReturnsExpectedQuery()
        {
            // act
            var query = DbSetHelpers.GetEntityByIdQuery(dbSet: _context.TestEntities, id: IdThatDoesNotExist, cancellationToken: TestContext.Current.CancellationToken);
            var result = await query.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
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

    private class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    private class TestEntity : BaseEntity
    {
        public override SystemEntityType SystemEntityType => SystemEntityTypes.Unspecified;
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
