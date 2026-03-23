using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Processes;

public class Process : SoftDeletableEntity, IStaticEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.Process;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public DateTime? StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? FaileddOn { get; set; }
    public string? Message { get; set; }

    public ProcessStatus Status
    {
        get
        {
            if (!StartedOn.HasValue) return ProcessStatus.NotStarted;
            if (FaileddOn.HasValue) return ProcessStatus.Failed;
            if (CompletedOn.HasValue) return ProcessStatus.Completed;
            return ProcessStatus.Started;
        }
    }

    public const int MaxLengthMessage = 250;
}