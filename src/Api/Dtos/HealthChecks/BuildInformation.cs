namespace Desic.Api.Dtos.HealthChecks;

public class BuildInformation
{
    public string? CommitSha { get; set; }
    public DateTime? CreatedOn { get; set; }
    public bool? IsCi { get; set; }
    public long? RunAttempt { get; set; }
    public long? RunId { get; set; }
    public long? RunNumber { get; set; }
    public string? Version { get; set; }
}
