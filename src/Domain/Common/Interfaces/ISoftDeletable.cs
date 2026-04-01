namespace Desic.Domain.Common.Interfaces;

public interface ISoftDeletable : IReadOnlySoftDeletable
{
    new Guid? DeletedById { get; set; }
    new string? DeletedByName { get; set; }
    new Guid? DeletedByTypeId { get; set; }
    new DateTime? DeletedOn { get; set; }
}
