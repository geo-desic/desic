using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Desic.Api.Dtos.HealthChecks;

public class HealthReport
{
    public BuildInformation Build { get; set; } = new();
    public HealthStatus OverallStatus { get; set; }
    public long TotalDurationMilliseconds { get; set; }
    public List<HealthReportEntry>? Entries { get; set; } = [];
}
