using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Desic.Api.Dtos.HealthChecks
{
    public class HealthReportEntry
    {
        public string? Name { get; set; }
        public HealthStatus Status { get; set; }
        public long DurationMilliseconds { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public IReadOnlyDictionary<string, object>? Data { get; set; }
    }
}
