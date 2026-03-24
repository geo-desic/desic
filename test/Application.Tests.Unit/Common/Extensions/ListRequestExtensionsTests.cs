using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class ListRequestExtensionsTests
{
    private readonly FakeLogger<ListRequestExtensionsTests> _logger = new();
    public const int DefaultLogEventId = 1;
    public const string LogMessagePatternNegativeOffset = $"^Negative {nameof(IListRequest.StartIndex)} is not supported";
    public const string LogMessagePatternNegativeCount = $"^Negative {nameof(IListRequest.Count)} is not supported";
    public const string LogMessagePatternCountGreaterThanMaximum = "^Requested count is greater than maximum allowed";

    public class ListRequestExtensionsTests001 : ListRequestExtensionsTests
    {
        [Theory]
        // no sanitation needed
        [InlineData(0, 0, 0, 0, 5)]   // count = 0 <= 5  ====> okay
        [InlineData(0, 1, 0, 1, 5)]   // count = 1 <= 5  ====> okay
        [InlineData(0, 5, 0, 5, 5)]   // count = 5 <= 5  ====> okay
        // sanitation needed
        [InlineData(0, 5, 0, 6, 5)]   // count = 6 > 5   ====> reduce to 5
        [InlineData(0, 0, -1, 0, 5)]  // startIndex = -1 ====> update to 0
        [InlineData(0, 0, 0, -1, 5)]  // count = -1 < 0  ====> update to 0
        public void Sanitize_SpecifiedData_ExpectedUpdatesAndLoggedMessages(int expectedStartIndex, int expectedCount, int startIndex, int count, int maximumAllowedCount)
        {
            // arrange
            var expectedLogMessageNegativeOffset = startIndex < 0;
            var expectedLogMessageNegativeCount = count < 0;
            var expectedLogMessageCountGreaterThanMaximum = count > maximumAllowedCount;
            var settings = new ListRequestSanitizationSettings
            {
                MaximumAllowedCount = maximumAllowedCount,
                Logger = _logger,
                LogEventId = DefaultLogEventId,
            };
            var request = new ListRequest
            {
                StartIndex = startIndex,
                Count = count,
            };

            // act
            ListRequestExtensions.Sanitize(request: request, settings: settings);

            // assert
            request.StartIndex.Should().Be(expectedStartIndex);
            request.Count.Should().Be(expectedCount);
            _logger.LogMessageExists(LogMessagePatternNegativeOffset, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageNegativeOffset);
            _logger.LogMessageExists(LogMessagePatternNegativeCount, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageNegativeCount);
            _logger.LogMessageExists(LogMessagePatternCountGreaterThanMaximum, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageCountGreaterThanMaximum);
        }
    }
}
