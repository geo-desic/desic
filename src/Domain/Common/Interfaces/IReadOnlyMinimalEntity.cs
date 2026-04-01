using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlyMinimalEntity : IReadOnlyGuidId
{
    SystemEntityType SystemEntityType { get; }
}
