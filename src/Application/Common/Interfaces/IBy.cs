using Desic.Domain.Common.Interfaces;

namespace Desic.Application.Common.Interfaces;

public interface IBy
{
    IReadOnlyMinimalEntity By { get; }
}
