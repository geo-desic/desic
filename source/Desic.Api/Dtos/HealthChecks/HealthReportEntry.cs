using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Desic.Api.Dtos.HealthChecks
{
    public class HealthReportEntry
    {
        public string? Description { get; set; }
        public long DurationMilliseconds { get; set; }
        public HealthStatus Status { get; set; }
    }
}
