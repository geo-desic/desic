using AwesomeAssertions;
using Desic.Shared.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Text.RegularExpressions;

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
            var behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger);
            var request = new TestRequest { Message = "Request" };
            var expectedResponse = new TestResponse { Message = "Response" };
            async Task<TestResponse> next(CancellationToken t = default) => expectedResponse;

            // act
            var response = await behavior.Handle(request, next, TestContext.Current.CancellationToken);

            // assert
            _logger.Collector.Count.Should().Be(4);
            LogMessageExists(_logger, $"^Handling {nameof(TestRequest)}$", LogLevel.Debug).Should().BeTrue();
            LogMessageExists(_logger, $"^Handled {nameof(TestRequest)} returning {nameof(TestResponse)} in \\d+\\.?\\d*ms$", LogLevel.Debug).Should().BeTrue();
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }

    private static bool LogMessageExists<T>(FakeLogger<T> logger, string messagePattern, LogLevel? level = null)
    {
        Func<FakeLogRecord, bool> predicate = level.HasValue ? x => Regex.IsMatch(x.Message, messagePattern) && x.Level == level : x => Regex.IsMatch(x.Message, messagePattern);
        return logger.Collector.GetSnapshot().Any(predicate);
    }
}
