using AwesomeAssertions;
using Desic.Domain.EntityTypes;
using Desic.Domain.Tags;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.EntityTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Infrastructure.Tests.Unit.Data.EntityTypes;

// note: the in memory sqlite provider used because the efcore one does not support DbSet<T>.ExecuteDelete which is used by the handler
public class SeedEntityTypesRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly FakeLogger<SeedEntityTypesRequestHandler> _logger = new();
    private readonly Tag _by = SystemTags.System.ToEntity();
    private readonly DbSet<EntityType> _dbSet;
    private readonly EntityType _seededEntity = SystemEntityTypes.Unspecified.ToEntity();

    private const string TableName = nameof(ApplicationDbContext.EntityTypes);

    public SeedEntityTypesRequestHandlerTests()
    {
        _dbSet = DbContext.EntityTypes;
    }

    public class SeedEntityTypesRequestHandlerTests001 : SeedEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingEntities_AllReferencedEntitiesSeeded(ApplicationDatabaseSeedingMethod method)
        {
            // arrange
            var expected = ExpectedEntities();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = method };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests002 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntity_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Fast };

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

    public class SeedEntityTypesRequestHandlerTests003 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatIsCorrect_AllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests004 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeUpdated_PerformsUpdateAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be updated
            _seededEntity.Name = "NeedsToBeUpdated";
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(1);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests005 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeDeleted_PerformsDeleteAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be deleted
            var seededEntity = new EntityType { Id = Guid.AllBitsSet, Key = "zzzz", Name = "NeedsToBeDeleted" };
            await Setup(seededEntity: seededEntity);
            var expected = ExpectedEntities();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(1);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public static List<EntityType> ExpectedEntities()
    {
        return [.. SystemEntityTypes.AllAsEntities()];
    }

    public async Task Setup(EntityType? seededEntity = null)
    {
        if (seededEntity != null)
        {
            _dbSet.Add(seededEntity);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }
}
