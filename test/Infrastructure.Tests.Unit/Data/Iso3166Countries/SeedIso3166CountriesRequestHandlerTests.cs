using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Iso3166Countries;
using Desic.Domain.Tags;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;
using Moq;

namespace Desic.Infrastructure.Tests.Unit.Data.Iso3166Countries;

public class SeedIso3166CountriesRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly Tag _by = SystemTags.System.ToEntity();
    private readonly DbSet<Iso3166Country> _dbSet;
    private readonly FakeLogger<SeedIso3166CountriesRequestHandler> _logger = new();
    private readonly Mock<IMediator> _mediator = new();
    private readonly Iso3166Country _seededEntity = EntityFromIndex(index: 1);

    private const string TableName = nameof(ApplicationDbContext.Iso3166Countries);
    private const int TotalReferencedEntities = 5;

    public SeedIso3166CountriesRequestHandlerTests()
    {
        // these need to exist due to foriegn key constaints
        _dbSet = DbContext.Iso3166Countries;
        _seededEntity.SetCreatedAndModifiedBy(_by);
        DbContext.EntityTypes.AddRange(SystemEntityTypes.Tag.ToEntity(), SystemEntityTypes.Iso3166Country.ToEntity());
        DbContext.Tags.Add(_by);
        DbContext.SaveChanges();
    }

    public class SeedIso3166CountriesRequestHandlerTests001 : SeedIso3166CountriesRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingIso3166Countries_AllReferencedEntitiesSeeded(ApplicationDatabaseSeedingMethod method)
        {
            // arrange
            await Setup();
            var expected = await ExpectedEntities(count: TotalReferencedEntities).ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
            var handler = new SeedIso3166CountriesRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object);
            var request = new SeedIso3166CountriesRequest { By = _by, Method = method };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedIso3166CountriesRequestHandlerTests002 : SeedIso3166CountriesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntity_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntity: _seededEntity);
            var handler = new SeedIso3166CountriesRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object);
            var request = new SeedIso3166CountriesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Fast };

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

    public class SeedIso3166CountriesRequestHandlerTests003 : SeedIso3166CountriesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatIsCorrect_AllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntity: _seededEntity);
            var expected = await ExpectedEntities(count: TotalReferencedEntities).ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
            var handler = new SeedIso3166CountriesRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object);
            var request = new SeedIso3166CountriesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedIso3166CountriesRequestHandlerTests004 : SeedIso3166CountriesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeUpdated_PerformsUpdateAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be updated
            _seededEntity.Name = "NeedsToBeUpdated";
            await Setup(seededEntity: _seededEntity);
            var expected = await ExpectedEntities(count: TotalReferencedEntities).ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
            var handler = new SeedIso3166CountriesRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object);
            var request = new SeedIso3166CountriesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(1);
            _dbSet.AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    public class SeedIso3166CountriesRequestHandlerTests005 : SeedIso3166CountriesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityThatNeedsToBeDeleted_PerformsDeleteAndAllOtherReferencedEntitiesSeeded()
        {
            // arrange
            // seed one that needs to be deleted
            var seededEntity = new Iso3166Country { Id = Guid.AllBitsSet, IsoId = -1, Alpha2 = "zz", Alpha3 = $"zzz", Name = "NeedsToBeDeleted" }; // non-existant in expected entities: IsoId = -1
            seededEntity.SetCreatedAndModifiedBy(_by);
            await Setup(seededEntity: seededEntity);
            var expected = await ExpectedEntities(count: TotalReferencedEntities).ToListAsync(cancellationToken: TestContext.Current.CancellationToken);
            var handler = new SeedIso3166CountriesRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object);
            var request = new SeedIso3166CountriesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            _logger.LogMessageExists($"^Seeded {TableName}").Should().BeTrue();
            result.Deletes.Should().Be(1);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            _dbSet.Where(x => !x.IsDeleted).AsEnumerable().Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Id).Excluding(x => x.CreatedOn).Excluding(x => x.ModifiedOn));
        }
    }

    private async IAsyncEnumerable<Iso3166Country> ExpectedEntities(int count)
    {
        for (var i = 1; i <= count; ++i)
        {
            var item = EntityFromIndex(index: i);
            item.SetCreatedAndModifiedBy(_by);
            yield return item;
        }
    }

    private static Iso3166Country EntityFromIndex(int index) => new() { Id = index.ToGuid(), IsoId = index, Alpha2 = $"a{index}", Alpha3 = $"aa{index}", Name = $"Country{index}" };

    private async Task Setup(Iso3166Country? seededEntity = null)
    {
        _mediator.Setup(x => x.CreateStream(It.IsAny<Iso3166CountriesResourceStreamRequest>(), It.IsAny<CancellationToken>())).Returns(ExpectedEntities(count: TotalReferencedEntities));
        if (seededEntity != null)
        {
            _dbSet.Add(seededEntity);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }
}
