using AwesomeAssertions;
using Desic.Domain.Processes;
using System.Globalization;

namespace Desic.Domain.Tests.Unit.Processes;

public class ProcessTests
{
    private const string DefaultStartedOnString = "2020-01-01T00:00:00";
    private const string DefaultCompletedOnString = "2020-01-01T02:00:00";
    private const string DefaultFailedOnString = "2020-01-01T01:00:00";
    public class ProcessTests001 : ProcessTests
    {
        [Theory]
        [InlineData(ProcessStatus.NotStarted, null, null, null)]
        [InlineData(ProcessStatus.Started, DefaultStartedOnString, null, null)]
        [InlineData(ProcessStatus.Failed, DefaultStartedOnString, DefaultFailedOnString, null)]
        [InlineData(ProcessStatus.Failed, DefaultStartedOnString, DefaultFailedOnString, DefaultCompletedOnString)]
        [InlineData(ProcessStatus.Completed, DefaultStartedOnString, null, DefaultCompletedOnString)]
        public void Process_WithSpecifiedDateValues_HasExpectedStatus(ProcessStatus expected, string? startedOnString, string? failedOnString, string? completedOnString)
        {
            // arrange
            var entity = new Process
            {
                StartedOn = NullableDateTimeFromString(startedOnString),
                CompletedOn = NullableDateTimeFromString(completedOnString),
                FaileddOn = NullableDateTimeFromString(failedOnString),
            };

            // act
            var status = entity.Status;

            // assert
            status.Should().Be(expected);
        }
    }

    private static DateTime? NullableDateTimeFromString(string? value)
    {
        if (value == null) return null;
        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }
}
