using AwesomeAssertions;
using Desic.Shared.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Shared.Tests.Unit.Mediator;

public class LoggingBehaviorTests
{
    private readonly FakeLogger<LoggingBehavior<TestRequest, TestResponse>> _logger = new();

    public class LoggingBehaviorTests001 : LoggingBehaviorTests
    {
        [Fact]
        public async Task Handle_SpecifiedRequest_ExpectedLogging()
        {
            // arrange
            var request = new TestRequest { Message = "Request" };
            var handler = new TestRequestHandler();
            var expectedResponse = new TestResponse { Message = "Request Response" };
            var behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger) { NextPipeline = handler };

            // act
            var response = await behavior.Handle(request, TestContext.Current.CancellationToken);

            // assert
            _logger.Collector.Count.Should().Be(4);
            _logger.LogMessageExists($"^Handling {nameof(TestRequest)}$", level: LogLevel.Debug).Should().BeTrue();
            _logger.LogMessageExists($"^Handled {nameof(TestRequest)} returning {nameof(TestResponse)} in \\d+\\.?\\d*ms$", level: LogLevel.Debug).Should().BeTrue();
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
