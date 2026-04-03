namespace Desic.Api.Dtos.HealthChecks;

public class BuildInformation
{
    public int? Attempt { get; set; }
    public string? CommitSha { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? Id { get; set; }
    public int? Number { get; set; }
    public string? Version { get; set; }
}
