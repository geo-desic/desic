using AwesomeAssertions;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.EntityTypes;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Infrastructure.Data.Labels;
using Desic.Infrastructure.Data.Test.Users;
using MediatR;
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
            DbContext.Labels.Any().Should().BeFalse();
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
            DbContext.Labels.Count().Should().Be(1);
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
            DbContext.Labels.Count().Should().BeGreaterThanOrEqualTo(1);
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
            DbContext.Labels.Count().Should().Be(1);
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
            DbContext.Labels.Count().Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedTestUsersRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Exactly(expectedTimes));
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedTestUsersRequestHandler));
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

    private void Setup()
    {
        _mediator.Setup(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedEntityTypesResult());
        _mediator.Setup(x => x.Send(It.IsAny<SeedLabelsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedLabelsResult());
        _mediator.Setup(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedIso3166CountriesResult());
        _mediator.Setup(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedTestUsersResult());
    }

    private static void VerifyAllSeedRequestHandlers(Mock<IMediator> mediator, Times times, Type? except = null)
    {
        if (except != typeof(SeedEntityTypesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedLabelsRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedLabelsRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedIso3166CountriesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedTestUsersRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>()), times);
    }
}
