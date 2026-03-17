using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using System.Text.RegularExpressions;

namespace Desic.Infrastructure.Tests.Unit;

internal static class FakeLoggerExtensions
{
    public static bool LogMessageExists<T>(this FakeLogger<T> logger, string messagePattern, LogLevel? level = null)
    {
        Func<FakeLogRecord, bool> predicate = level.HasValue ? x => Regex.IsMatch(x.Message, messagePattern) && x.Level == level : x => Regex.IsMatch(x.Message, messagePattern);
        return logger.Collector.GetSnapshot().Any(predicate);
    }
}
