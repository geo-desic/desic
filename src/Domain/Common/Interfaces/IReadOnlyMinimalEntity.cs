using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlyMinimalEntity : IReadOnlyId
{
    SystemEntityType SystemEntityType { get; }
}
