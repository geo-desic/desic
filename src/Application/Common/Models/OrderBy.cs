using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class OrderBy<T> : IOrderBy<T> where T : struct, Enum
{
    public bool Ascending { get; set; } = true;
    public T Property { get; set; }
}
