namespace Desic.Domain.Common.Interfaces;

public interface IModifiable
{
    Guid ModifiedById { get; set; }
    string? ModifiedByName { get; set; }
    Guid ModifiedByTypeId { get; set; }
    DateTime ModifiedOn { get; set; }
}
