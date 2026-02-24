namespace Desic.Core.Shared.Entities;

public interface IModifiable
{
    Guid ModifiedById { get; set; }
    Guid ModifiedByTypeId { get; set; }
    DateTime ModifiedOn { get; set; }
}
