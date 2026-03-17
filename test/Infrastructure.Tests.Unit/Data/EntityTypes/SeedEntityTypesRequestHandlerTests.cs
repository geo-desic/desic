using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using Desic.Domain.Tags;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.EntityTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Text.RegularExpressions;

namespace Desic.Infrastructure.Tests.Unit.Data.EntityTypes;

// note: the in memory sqlite provider used because the efcore one does not support DbSet<T>.ExecuteDelete which is used by the handler
public class SeedEntityTypesRequestHandlerTests : ApplicationDbContextImSqliteDependencyTests
{
    private readonly FakeLogger<SeedEntityTypesRequestHandler> _logger = new();
    private readonly IReadOnlyMinimalEntity _by = SystemTags.System;
    private readonly EntityType _seededEntityType = SystemEntityTypes.Unspecified.ToEntity();

    public class SeedEntityTypesRequestHandlerTests001 : SeedEntityTypesRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SpecifiedSeedingMethodWithNoExistingEntityTypes_AllReferencedEntityTypesSeeded(ApplicationDatabaseSeedingMethod method)
        {
            // arrange
            var expected = ExpectedEntityTypes();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = method };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            LogMessageExists(_logger, $"^Seeded {nameof(ApplicationDbContext.EntityTypes)}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            DbContext.EntityTypes.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests002 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFastWithExistingEntityType_SkipsSeeding()
        {
            // arrange
            // seed one so fast method should skip seeding altogether
            await Setup(seededEntityType: _seededEntityType);
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Fast };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            LogMessageExists(_logger, $"^Skipping {nameof(ApplicationDbContext.EntityTypes)} as it already has records$").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(0);
            result.ReferenceCount.Should().Be(0);
            result.Updates.Should().Be(0);
        }
    }

    public class SeedEntityTypesRequestHandlerTests003 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityTypeThatIsCorrect_AllOtherReferencedRecordsSeeded()
        {
            // arrange
            // seed one that is already correct (i.e. does not need to be updated or deleted)
            await Setup(seededEntityType: _seededEntityType);
            var expected = ExpectedEntityTypes();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            LogMessageExists(_logger, $"^Seeded {nameof(ApplicationDbContext.EntityTypes)}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            DbContext.EntityTypes.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests004 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityTypeThatNeedsToBeUpdated_PerformsUpdateAndAllOtherReferencedEntityTypesSeeded()
        {
            // arrange
            // seed one that needs to be updated
            _seededEntityType.Name = "NeedsToBeUpdated";
            await Setup(seededEntityType: _seededEntityType);
            var expected = ExpectedEntityTypes();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            LogMessageExists(_logger, $"^Seeded {nameof(ApplicationDbContext.EntityTypes)}").Should().BeTrue();
            result.Deletes.Should().Be(0);
            result.Inserts.Should().Be(expected.Count - 1);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(1);
            DbContext.EntityTypes.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public class SeedEntityTypesRequestHandlerTests005 : SeedEntityTypesRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingMethodFullWithExistingEntityTypeThatNeedsToBeDeleted_PerformsDeleteAndAllOtherReferencedEntityTypesSeeded()
        {
            // arrange
            // seed one that needs to be deleted
            var seededEntityType = new EntityType { Id = Guid.AllBitsSet, Key = "zzzz", Name = "NeedsToBeDeleted" };
            await Setup(seededEntityType: seededEntityType);
            var expected = ExpectedEntityTypes();
            var handler = new SeedEntityTypesRequestHandler(context: DbContext, logger: _logger);
            var request = new SeedEntityTypesRequest { By = _by, Method = ApplicationDatabaseSeedingMethod.Full };

            // act
            var result = await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            LogMessageExists(_logger, $"^Seeded {nameof(ApplicationDbContext.EntityTypes)}").Should().BeTrue();
            result.Deletes.Should().Be(1);
            result.Inserts.Should().Be(expected.Count);
            result.ReferenceCount.Should().Be(expected.Count);
            result.Updates.Should().Be(0);
            DbContext.EntityTypes.AsEnumerable().Should().BeEquivalentTo(expected);
        }
    }

    public static List<EntityType> ExpectedEntityTypes()
    {
        return [.. SystemEntityTypes.AllAsEntities()];
    }

    public async Task Setup(EntityType? seededEntityType = null)
    {
        if (seededEntityType != null)
        {
            DbContext.EntityTypes.Add(seededEntityType);
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
        }
    }

    private static bool LogMessageExists<T>(FakeLogger<T> logger, string messagePattern, LogLevel? level = null)
    {
        Func<FakeLogRecord, bool> predicate = level.HasValue ? x => Regex.IsMatch(x.Message, messagePattern) && x.Level == level : x => Regex.IsMatch(x.Message, messagePattern);
        return logger.Collector.GetSnapshot().Any(predicate);
    }
}
