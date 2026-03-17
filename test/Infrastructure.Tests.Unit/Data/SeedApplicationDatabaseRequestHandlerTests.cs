using AwesomeAssertions;
using Desic.Infrastructure.Data;
using Desic.Infrastructure.Data.EntityTypes;
using Desic.Infrastructure.Data.Iso3166Countries;
using Desic.Infrastructure.Data.Tags;
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
            DbContext.Tags.Any().Should().BeFalse();
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never());
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests002 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SeedingOnlyEntityTypesEnabled_OnlyExpectedSeedingOccurs(ApplicationDatabaseSeedingMethod expectedMethod)
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
            DbContext.Tags.Count().Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedEntityTypesRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedEntityTypesRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests003 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SeedingOnlyTagsEnabled_OnlyExpectedSeedingOccurs(ApplicationDatabaseSeedingMethod expectedMethod)
        {
            // arrange
            Setup();
            var options = NewOptions();
            options.Value.Tags!.Enabled = true;
            options.Value.Tags!.Method = expectedMethod;
            var handler = new SeedApplicationDatabaseRequestHandler(context: DbContext, logger: _logger, mediator: _mediator.Object, seedingOptions: options);
            var request = new SeedApplicationDatabaseRequest();

            // act
            await handler.Handle(request: request, cancellationToken: TestContext.Current.CancellationToken);

            // assert
            DbContext.Tags.Count().Should().BeGreaterThanOrEqualTo(1);
            _mediator.Verify(x => x.Send(It.Is<SeedTagsRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedTagsRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests004 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full)]
        public async Task Handle_SeedingOnlyIso3166CountriesEnabled_OnlyExpectedSeedingOccurs(ApplicationDatabaseSeedingMethod expectedMethod)
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
            DbContext.Tags.Count().Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedIso3166CountriesRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Once());
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedIso3166CountriesRequestHandler));
        }
    }

    public class SeedApplicationDatabaseRequestHandlerTests005 : SeedApplicationDatabaseRequestHandlerTests
    {
        [Theory]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast, false)]
        [InlineData(ApplicationDatabaseSeedingMethod.Fast, true)]
        [InlineData(ApplicationDatabaseSeedingMethod.Full, true)]
        public async Task Handle_SeedingOnlyTestUsersEnabled_OnlyExpectedSeedingOccurs(ApplicationDatabaseSeedingMethod expectedMethod, bool testEnabled)
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
            DbContext.Tags.Count().Should().Be(1);
            _mediator.Verify(x => x.Send(It.Is<SeedTestUsersRequest>(x => x.Method == expectedMethod), It.IsAny<CancellationToken>()), Times.Exactly(expectedTimes));
            VerifyAllSeedRequestHandlers(mediator: _mediator, Times.Never(), except: typeof(SeedTestUsersRequestHandler));
        }
    }

    private static IOptions<ApplicationDatabaseSeedingOptions> NewOptions(bool enabled = true, bool testEnabled = true, bool individualEntitiesEnabled = false)
    {
        var options = new ApplicationDatabaseSeedingOptions
        {
            Enabled = enabled,
            EntityTypes = new ApplicationDatabaseSeedingEntityTypesOptions { Enabled = individualEntitiesEnabled },
            Iso3166Countries = new ApplicationDatabaseSeedingIso3166CountriesOptions { Enabled = individualEntitiesEnabled },
            Tags = new ApplicationDatabaseSeedingTagsOptions { Enabled = individualEntitiesEnabled },
            Test = new ApplicationDatabaseSeedingTestOptions
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
        _mediator.Setup(x => x.Send(It.IsAny<SeedTagsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedTagsResult());
        _mediator.Setup(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedIso3166CountriesResult());
        _mediator.Setup(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SeedTestUsersResult());
    }

    private static void VerifyAllSeedRequestHandlers(Mock<IMediator> mediator, Times times, Type? except = null)
    {
        if (except != typeof(SeedEntityTypesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedEntityTypesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedTagsRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedTagsRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedIso3166CountriesRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedIso3166CountriesRequest>(), It.IsAny<CancellationToken>()), times);
        if (except != typeof(SeedTestUsersRequestHandler)) mediator.Verify(x => x.Send(It.IsAny<SeedTestUsersRequest>(), It.IsAny<CancellationToken>()), times);
    }
}
