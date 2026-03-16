using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Interfaces;

public interface IBy
{
    IReadOnlyMinimalEntity By { get; }
}
