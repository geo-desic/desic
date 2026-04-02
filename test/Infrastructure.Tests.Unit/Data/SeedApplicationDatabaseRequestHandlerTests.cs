using AwesomeAssertions;
using Desic.Domain.EntityTypes;
using Desic.Domain.Labels;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.EntityTypes;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Infrastructure.Data.Labels;
using Desic.Infrastructure.Data.Test.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Desic.Infrastructure.Tests.Unit.Data;

public class SeedApplicationDatabaseRequestHandlerTests : ApplicationDbContextImEfCoreDependencyTests
{
    private readonly ILogger<SeedApplicationDatabaseRequestHandler> _logger = NullLogger<SeedApplicationDatabaseRequestHandler>.Instance;
    private readonly Mock<IMediator> _mediator = new();

    public class SeedApplicationDatabaseRequestHandlerTests001 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Fact]
        public async Task Handle_SeedingNotEnabled_NoSeedingOccurs()
        {
            // arrange
            Setup();
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: NewOptions(enabled: false));
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            (await DbContext.Processes.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(false);
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never());
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests002 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SeedingOnlyEntityTypesEnabled_OnlyExpectedSeedingOccurs(SeedApplicationDatabaseMethod expectedMethod)
        {
            // arrange
            Setup();
            var options = NewOptions();
            options.Value.EntityTypes!.Enabled = true;
            options.Value.EntityTypes!.Method = expectedMethod;
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            (await DbContext.Processes.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedEntityTypesRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedEntityTypesRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests003 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SeedingOnlyLabelsEnabled_OnlyExpectedSeedingOccurs(SeedApplicationDatabaseMethod expectedMethod)
        {
            // arrange
            Setup();
            var options = NewOptions();
            options.Value.Labels!.Enabled = true;
            options.Value.Labels!.Method = expectedMethod;
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            (await DbContext.Processes.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedLabelsRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedLabelsRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests004 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast)]
        [InlineData(SeedApplicationDatabaseMethod.Full)]
        public async Task Handle_SeedingOnlyIso3166CountriesEnabled_OnlyExpectedSeedingOccurs(SeedApplicationDatabaseMethod expectedMethod)
        {
            // arrange
            Setup();
            var options = NewOptions();
            options.Value.Iso3166Countries!.Enabled = true;
            options.Value.Iso3166Countries!.Method = expectedMethod;
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            (await DbContext.Processes.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedIso3166CountriesRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedIso3166CountriesRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests005 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(SeedApplicationDatabaseMethod.Fast, false)]
        [InlineData(SeedApplicationDatabaseMethod.Fast, true)]
        [InlineData(SeedApplicationDatabaseMethod.Full, true)]
        public async Task Handle_SeedingOnlyTestUsersEnabled_OnlyExpectedSeedingOccurs(SeedApplicationDatabaseMethod expectedMethod, bool testEnabled)
        {
            // arrange
            Setup();
            var expectedTimes = testEnabled ? 1 : 0;
            var options = NewOptions(testEnabled: testEnabled);
            options.Value.Test!.Users!.Enabled = true;
            options.Value.Test!.Users!.Method = expectedMethod;
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            (await DbContext.Processes.CountAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedTestUsersRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Exactly(expectedTimes));
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedTestUsersRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests006 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ExceptionOccursBeforeProcessDependenciesPersisted_NoProcessPersisted()
        {
            // arrange
            var expectedException = new TestException("Test exception");
            Setup(includeSeedEntityTypes: false);
            _mediator.Setup(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception: expectedException);
            var options = NewOptions(individualEntitiesEnabled: true);
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();
            var act = async () => await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // act & assert
            await act.Should().ThrowAsync<TestException>().WithMessage(expectedException.Message);
            (await DbContext.Processes.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).Should().Be(false);
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests007 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ExceptionOccursAfterProcessDependenciesPersisted_ProcessPersistedWithExpectedMessage()
        {
            // arrange
            var expectedException = new TestException("Test exception");
            Setup(includeSeedIso3166Countries: false);
            _mediator.Setup(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception: expectedException);
            var options = NewOptions(individualEntitiesEnabled: true);
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // seed process dependencies to ensure process can be persisted
            DbContext.EntityTypes.AddRange(SystemEntityTypes.AllAsEntities());
            DbContext.Labels.Add(SystemLabels.System.ToEntity());
            await DbContext.SaveChangesAsync(cancellationToken: TestContext.Current.CancellationToken);
            DbContext.ChangeTracker.Clear();

            var act = async () => await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // act & assert
            await act.Should().ThrowAsync<TestException>().WithMessage(expectedException.Message);
            var processCreated = await DbContext.Processes.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
            processCreated.Should().NotBeNull();
            processCreated.Message.Should().Be(expectedException.Message);
        }
    }

    private static IOptions<SeedApplicationDatabaseOptions> NewOptions(bool enabled = true, bool testEnabled = true, bool individualEntitiesEnabled = false)
    {
        var options = new SeedApplicationDatabaseOptions
        {
            Enabled = enabled,
            EntityTypes = new SeedApplicationDatabaseEntityTypesOptions { Enabled = individualEntitiesEnabled },
            Iso3166Countries = new SeedApplicationDatabaseIso3166CountriesOptions { Enabled = individualEntitiesEnabled },
            Labels = new SeedApplicationDatabaseLabelsOptions { Enabled = individualEntitiesEnabled },
            Test = new SeedApplicationDatabaseTestOptions
            {
                Enabled = testEnabled,
                Users = new ApplicationDatabaseSeedingTestUsersOptions { Enabled = individualEntitiesEnabled },
            },
        };
        return Options.Create(options);
    }

    private void Setup(bool includeSeedEntityTypes = true, bool includeSeedLabels = true, bool includeSeedIso3166Countries = true, bool includeSeedTestUsers = true)
    {
        if (includeSeedEntityTypes) _mediator.Setup(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedEntityTypesResult());
        if (includeSeedLabels) _mediator.Setup(x => x.Send(It.IsAny<SeedLabelsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedLabelsResult());
        if (includeSeedIso3166Countries) _mediator.Setup(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedIso3166CountriesResult());
        if (includeSeedTestUsers) _mediator.Setup(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedTestUsersResult());
    }

    private static void VerifyAllSeedRequestHandlers(Mock<IMediator> mediator, Times times, Type? except = null)
    {
        if (except != typeof(SeedEntityTypesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedLabelsRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedLabelsRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedIso3166CountriesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedTestUsersRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>()), times);
    }

    public class TestException(string? message) : Exception(message) { }
}
