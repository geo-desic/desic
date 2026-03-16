using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class ByRequest : IBy
{
    public required IReadOnlyMinimalEntity By { get; set; }
}
