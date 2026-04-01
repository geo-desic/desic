using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.Common.Entities;

public abstract class SoftDeletableEntity : ModifiableEntity, ISoftDeletableEntity
{
    public Guid? DeletedById { get; set; }
    public string? DeletedByName { get; set; }
    public Guid? DeletedByTypeId { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool IsDeleted => DeletedOn.HasValue;

    public const int MaxLengthDeletedByName = 100;
}
