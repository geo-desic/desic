using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Microsoft.Extensions.Logging.Testing;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class PaginationExtensionsTests
{
    private readonly FakeLogger<PaginationExtensionsTests> _logger = new();
    public const int DefaultLogEventId = 1;
    public const string LogMessagePatternNegativeOffset = $"^Negative {nameof(IPagination.StartIndex)} is not supported";
    public const string LogMessagePatternNegativeCount = $"^Negative {nameof(IPagination.Count)} is not supported";
    public const string LogMessagePatternCountGreaterThanMaximum = "^Requested count is greater than maximum allowed";
    public const string LogMessageIncludeTotalCountNotAllowed = $"^Including total count is not allowed";

    public class PaginationExtensionsTests001 : PaginationExtensionsTests
    {
        [Theory]
        // no sanitation needed
        [InlineData(0, 0, 0, 0, 5, false, false)]   // count = 0 <= 5                           ====> okay
        [InlineData(0, 1, 0, 1, 5, false, false)]   // count = 1 <= 5                           ====> okay
        [InlineData(0, 5, 0, 5, 5, false, false)]   // count = 5 <= 5                           ====> okay
        [InlineData(0, 0, 0, 0, 0, true, true)]     // includeTotalCount when it is allowed     ====> okay
        // sanitation needed
        [InlineData(0, 5, 0, 6, 5, false, false)]   // count = 6 > 5                            ====> reduce to 5
        [InlineData(0, 0, -1, 0, 5, false, false)]  // startIndex = -1                          ====> update to 0
        [InlineData(0, 0, 0, -1, 5, false, false)]  // count = -1 < 0                           ====> update to 0
        [InlineData(0, 0, 0, 0, 0, true, false)]    // includeTotalCount when it is not allowed ====> change to false
        public void Sanitize_SpecifiedData_ExpectedUpdatesAndLoggedMessages(int expectedStartIndex, int expectedCount, int startIndex, int count, int maximumAllowedCount, bool includeTotalCount, bool includeTotalCountAllowed)
        {
            // arrange
            var expectedIncludeTotalCount = includeTotalCount && includeTotalCountAllowed;
            var expectedLogMessageNegativeOffset = startIndex < 0;
            var expectedLogMessageNegativeCount = count < 0;
            var expectedLogMessageCountGreaterThanMaximum = count > maximumAllowedCount;
            var expectedLogMessageIncludeTotalCountNotAllowed = !includeTotalCountAllowed && includeTotalCount;
            var settings = new PaginationSanitizationSettings
            {
                IncludeTotalCountAllowed = includeTotalCountAllowed,
                MaximumAllowedCount = maximumAllowedCount,
                Logger = _logger,
                LogEventId = DefaultLogEventId,
            };
            var source = new Pagination
            {
                Count = count,
                IncludeTotalCount = includeTotalCount,
                StartIndex = startIndex,
            };

            // act
            PaginationExtensions.Sanitize(source: source, settings: settings);

            // assert
            source.Count.Should().Be(expectedCount);
            source.IncludeTotalCount.Should().Be(expectedIncludeTotalCount);
            source.StartIndex.Should().Be(expectedStartIndex);
            _logger.LogMessageExists(LogMessagePatternNegativeOffset, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageNegativeOffset);
            _logger.LogMessageExists(LogMessagePatternNegativeCount, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageNegativeCount);
            _logger.LogMessageExists(LogMessagePatternCountGreaterThanMaximum, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageCountGreaterThanMaximum);
            _logger.LogMessageExists(LogMessageIncludeTotalCountNotAllowed, eventId: settings.LogEventId.Id).Should().Be(expectedLogMessageIncludeTotalCountNotAllowed);
        }
    }
}
