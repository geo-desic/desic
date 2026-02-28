using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Entities;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }

    SystemEntityType SystemEntityType { get; }
}
