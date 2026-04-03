namespace Desic.Api.Dtos.HealthChecks;

public class BuildInformation
{
    public string? CommitSha { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? RunAttempt { get; set; }
    public int? RunId { get; set; }
    public int? RunNumber { get; set; }
    public string? Version { get; set; }
}
