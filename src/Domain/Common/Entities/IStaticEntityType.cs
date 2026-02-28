using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Entities;

public interface IStaticEntityType
{
    static abstract SystemEntityType ClassEntityType { get; }
}
