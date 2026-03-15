using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class DbSetHelpersTests
{
    private readonly Mock<ITestDbContext> _mockDbContext = new();
    private readonly ITestDbContext _context;
    private readonly Guid IdThatDoesNotExist = Guid.AllBitsSet;
    private readonly Guid IdThatExists = 2.ToGuid(); // purposely using the id in the middle of the seeded data to make sure implementation actually finds the entity with the id versus simply returning the first or last one
    private const int TotalEntityCount = 3;

    public DbSetHelpersTests()
    {
        _mockDbContext.Setup(x => x.TestEntities).ReturnsDbSet(GetTestEntities(minIndex: 1, maxIndex: TotalEntityCount));
        _context = _mockDbContext.Object;
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
}
