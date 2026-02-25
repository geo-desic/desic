namespace Desic.Domain.Common.Entities;

public abstract class SeedableSoftDeletableEntity : SoftDeletableEntity
{
    public bool IsBeingSeeded
    {
        get => _isBeingSeeded ?? false;
        set => _isBeingSeeded = value;
    }
    private bool? _isBeingSeeded;
}
