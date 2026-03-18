using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Tags;
using Desic.Domain.Users;
using Desic.Domain.Users.Test;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Test.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Infrastructure.Tests.Unit.Data.Test.Users;

public class SeedTestUsersRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly Tag _by = SystemTags.System.ToEntity();
    private readonly DbSet<User> _dbSet;
    private readonly List<User> _expectedEntities;
    private readonly FakeLogger<SeedTestUsersRequestHandler> _logger = new();
    private readonly User _seededEntity;

    private const string TableName = nameof(ApplicationDbContext.Users);
    private const int TotalReferencedEntities = 10;

    public SeedTestUsersRequestHandlerTests()
    {
        // these need to exist due to foriegn key constaints
        _dbSet = DbContext.Users;
        _expectedEntities = ExpectedEntities().GetAwaiter().GetResult();
        _seededEntity = EntityFromIndex(index: 0);
        DbContext.EntityTypes.AddRange(SystemEntityTypes.Tag.ToEntity(), SystemEntityTypes.User.ToEntity());
        DbContext.Tags.Add(_by);
        DbContext.SaveChanges();
    }

    public class SeedTestUsersRequestHandlerTests001 : SeedTestUsersRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingEntities_AllReferencedEntitiesSeeded(SeedApplicationDatabaseMethod method)
        {
            // arrange
            await Setup();
            var handler = new SeedTestUsersRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTestUsersRequest { By = _by, Method = method, Count = TotalReferencedEntities };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(_expectedEntities.Count);
            result.ReferenceCount.Should().Be(_expectedEntities.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(_expectedEntities, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedTestUsersRequestHandlerTests002 : SeedTestUsersRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntity_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedTestUsersRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTestUsersRequest { By = _by, Method = SeedApplicationDatabaseMethod.Fast, Count = TotalReferencedEntities };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Skipping {TableName} as it already has records$").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(0);
            result.ReferenceCount.Should().Be(0);
            result.Updates.Should().Be(0);
        }
    }

    public class SeedTestUsersRequestHandlerTests003 : SeedTestUsersRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatIsCorrect_AllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedTestUsersRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTestUsersRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full, Count = TotalReferencedEntities };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(_expectedEntities.Count - 1);
            result.ReferenceCount.Should().Be(_expectedEntities.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(_expectedEntities);
        }
    }

    public class SeedTestUsersRequestHandlerTests004 : SeedTestUsersRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatHasBeenUpdated_DoesNotUpdateAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that has been updated outside of seeding - it should not be updated by seeding
            _seededEntity.Username = "username-updated";
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedTestUsersRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTestUsersRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full, Count = TotalReferencedEntities };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(_expectedEntities.Count - 1);
            result.ReferenceCount.Should().Be(_expectedEntities.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(_expectedEntities); // _expectedEntities[0] is the updated entity
        }
    }

    public class SeedTestUsersRequestHandlerTests005 : SeedTestUsersRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingNonSeededEntity_DoesNotDeleteAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one additional non-system entity that should not be deleted
            var seededEntity = new User { Id = Guid.AllBitsSet, Username = "username-extra" }; // should not match any system entities due to Id and Username values
            seededEntity.SetCreatedAndModifiedBy(_by);
            await Setup(seededEntity: seededEntity);
            _expectedEntities.Add(seededEntity);
            var handler = new SeedTestUsersRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTestUsersRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full, Count = TotalReferencedEntities };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(_expectedEntities.Count - 1); // since we added one extra to expected
            result.ReferenceCount.Should().Be(_expectedEntities.Count - 1); // since we added one extra to expected
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(_expectedEntities);
        }
    }

    private async static Task<List<User>> ExpectedEntities(int count = TotalReferencedEntities)
    {
        return await TestUsers.Generate(count: count).ToListAsync();
    }

    private User EntityFromIndex(int index) => _expectedEntities[index];

    private async Task Setup(User? seededEntity = null)
    {
        if (seededEntity != null)
        {
            _dbSet.Add(seededEntity);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }
}
