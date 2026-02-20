namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public interface IModifiable
{
    Guid ModifiedById { get; set; }
    Guid ModifiedByTypeId { get; set; }
    DateTime ModifiedOn { get; set; }
}
