namespace Desic.Domain.Common.Interfaces;

public interface IModifiable : IReadOnlyModifiable
{
    new Guid ModifiedById { get; set; }
    new string? ModifiedByName { get; set; }
    new Guid ModifiedByTypeId { get; set; }
    new DateTime ModifiedOn { get; set; }
}
