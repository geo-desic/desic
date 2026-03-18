using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Text.RegularExpressions;

namespace Desic.Shared.Tests.Unit;

internal static class FakeLoggerExtensions
{
    public static bool LogMessageExists<T>(this FakeLogger<T> logger, string messagePattern, int? eventId = null, LogLevel? level = null)
    {
        bool predicate(FakeLogRecord x) => Regex.IsMatch(x.Message, messagePattern) && (eventId == null || x.Id == eventId) && (level == null || x.Level == level);
        return logger.Collector.GetSnapshot().Any(predicate);
    }
}
