using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Interfaces;

public interface IStaticEntityType
{
    static abstract SystemEntityType ClassEntityType { get; }
}
