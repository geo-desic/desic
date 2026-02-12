namespace Desic.EntityFrameworkCore.Entities;

public abstract class SoftDeletableEntity : ModifiableEntity
{
    public bool IsDeleted
    {
        get => _iaDeleted ?? false;
        set => _iaDeleted = value;
    }
    private bool? _iaDeleted;
    public Guid DeletedById { get; set; }
    public Guid DeletedByTypeId { get; set; }
    public DateTime? DeletedOn { get; set; }
}
