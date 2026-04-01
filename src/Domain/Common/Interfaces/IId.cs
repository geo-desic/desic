namespace Desic.Domain.Common.Interfaces;

public interface IId : IReadOnlyId
{
    new Guid Id { get; set; }
}
