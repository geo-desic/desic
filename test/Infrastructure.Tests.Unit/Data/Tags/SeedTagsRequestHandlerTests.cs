using AwesomeAssertions;
using Desic.Application.Common;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Tags;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Infrastructure.Tests.Unit.Data.Tags;

public class SeedTagsRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly EntityType _by = SystemEntityTypes.EntityType.ToEntity(); // using this so no Tags need to be seeded that conflict with things being tested
    private readonly DbSet<Tag> _dbSet;
    private readonly FakeLogger<SeedTagsRequestHandler> _logger = new();
    private readonly Tag _seededEntity = SystemTags.System.ToEntity();

    private const int LogEventId = LogEvents.SeedTags;
    private const string TableName = nameof(ApplicationDbContext.Tags);

    public SeedTagsRequestHandlerTests()
    {
        // these need to exist due to foriegn key constaints
        _dbSet = DbContext.Tags;
        DbContext.EntityTypes.AddRange(SystemEntityTypes.Tag.ToEntity(), _by);
        DbContext.SaveChanges();

    }

    public class SeedTagsRequestHandlerTests001 : SeedTagsRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingEntities_AllReferencedEntitiesSeeded(SeedApplicationDatabaseMethod method)
        {
            // arrange
            await Setup();
            var expected = ExpectedEntities().ToList();
            var handler = new SeedTagsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTagsRequest { By = _by, Method = method };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedTagsRequestHandlerTests002 : SeedTagsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntity_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedTagsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTagsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Fast };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Skipping {TableName} as it already has records$", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(0);
            result.ReferenceCount.Should().Be(0);
            result.Updates.Should().Be(0);
        }
    }

    public class SeedTagsRequestHandlerTests003 : SeedTagsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatIsCorrect_AllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities().ToList();
            var handler = new SeedTagsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTagsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedTagsRequestHandlerTests004 : SeedTagsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeUpdated_PerformsUpdateAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be updated
            _seededEntity.Name = "NeedsToBeUpdated";
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities().ToList();
            var handler = new SeedTagsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTagsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(1);
            // note excluding ModifiedById and ModifiedByTypeId because we are using a non-normal _by above (not a Tag)
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn).Excluding(x => x.ModifiedById).Excluding(x => x.ModifiedByTypeId));
        }
    }

    public class SeedTagsRequestHandlerTests005 : SeedTagsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingNonSeededEntity_DoesNotDeleteAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one additional non-seeded entity that should not be deleted
            var seededEntity = new Tag { Id = Guid.AllBitsSet, Name = "ShouldNotBeDeleted" }; // should not match any seeded entities due to Id value
            seededEntity.SetCreatedAndModifiedBy(_by);
            await Setup(seededEntity: seededEntity);
            var expected = ExpectedEntities().ToList();
            expected.Add(seededEntity);
            var handler = new SeedTagsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedTagsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1); // since we added one extra to expected
            result.ReferenceCount.Should().Be(expected.Count - 1); // since we added one extra to expected
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    private static IEnumerable<Tag> ExpectedEntities() => SystemTags.AllAsEntities();

    private async Task Setup(Tag? seededEntity = null)
    {
        if (seededEntity != null)
        {
            _dbSet.Add(seededEntity);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }

}
