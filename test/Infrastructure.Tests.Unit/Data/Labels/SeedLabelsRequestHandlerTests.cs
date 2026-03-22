using AwesomeAssertions;
using Desic.Application.Common;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Labels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Infrastructure.Tests.Unit.Data.Labels;

public class SeedLabelsRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly EntityType _by = SystemEntityTypes.EntityType.ToEntity(); // using this so no Labels need to be seeded that conflict with things being tested
    private readonly DbSet<Label> _dbSet;
    private readonly FakeLogger<SeedLabelsRequestHandler> _logger = new();
    private readonly Label _seededEntity = SystemLabels.System.ToEntity();

    private const int LogEventId = LogEvents.SeedLabels;
    private const string TableName = nameof(ApplicationDbContext.Labels);

    public SeedLabelsRequestHandlerTests()
    {
        // these need to exist due to foriegn key constaints
        _dbSet = DbContext.Labels;
        DbContext.EntityTypes.AddRange(SystemEntityTypes.Label.ToEntity(), _by);
        DbContext.SaveChanges();

    }

    public class SeedLabelsRequestHandlerTests001 : SeedLabelsRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingEntities_AllReferencedEntitiesSeeded(SeedApplicationDatabaseMethod method)
        {
            // arrange
            await Setup();
            var expected = ExpectedEntities().ToList();
            var handler = new SeedLabelsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedLabelsRequest { By = _by, Method = method };

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

    public class SeedLabelsRequestHandlerTests002 : SeedLabelsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntity_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedLabelsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedLabelsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Fast };

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

    public class SeedLabelsRequestHandlerTests003 : SeedLabelsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatIsCorrect_AllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities().ToList();
            var handler = new SeedLabelsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedLabelsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

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

    public class SeedLabelsRequestHandlerTests004 : SeedLabelsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeUpdated_PerformsUpdateAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be updated
            _seededEntity.Name = "NeedsToBeUpdated";
            await Setup(seededEntity: _seededEntity);
            var expected = ExpectedEntities().ToList();
            var handler = new SeedLabelsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedLabelsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}", eventId: LogEventId).Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(1);
            // note excluding ModifiedBy* properties because we are using a non-normal _by above (not a Label)
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn).Excluding(x => x.ModifiedById).Excluding(x => x.ModifiedByName).Excluding(x => x.ModifiedByTypeId));
        }
    }

    public class SeedLabelsRequestHandlerTests005 : SeedLabelsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingNonSeededEntity_DoesNotDeleteAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one additional non-seeded entity that should not be deleted
            var seededEntity = new Label { Id = Guid.AllBitsSet, Name = "ShouldNotBeDeleted" }; // should not match any seeded entities due to Id value
            seededEntity.SetCreatedAndModifiedBy(_by);
            await Setup(seededEntity: seededEntity);
            var expected = ExpectedEntities().ToList();
            expected.Add(seededEntity);
            var handler = new SeedLabelsRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedLabelsRequest { By = _by, Method = SeedApplicationDatabaseMethod.Full };

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

    private static IEnumerable<Label> ExpectedEntities() => SystemLabels.AllAsEntities();

    private async Task Setup(Label? seededEntity = null)
    {
        if (seededEntity != null)
        {
            _dbSet.Add(seededEntity);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }

}
