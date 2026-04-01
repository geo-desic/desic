namespace Desic.Domain.Common.Interfaces;

public interface IGuidId : IReadOnlyGuidId
{
    new Guid Id { get; set; }
}
