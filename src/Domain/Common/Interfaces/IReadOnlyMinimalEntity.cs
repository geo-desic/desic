using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }

    SystemEntityType SystemEntityType { get; }
}
