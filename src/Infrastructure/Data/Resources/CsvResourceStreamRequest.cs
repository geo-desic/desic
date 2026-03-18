using MediatR;
using System.Reflection;
using System.Text;

namespace Desic.Infrastructure.Data.Resources;

public class CsvResourceStreamRequest<T> : IStreamRequest<T>
{
    public required Assembly Assembly { get; set; }
    public Type? ClassMapType { get; set; }
    public Encoding? Encoding { get; set; }
    public required string ResourceName { get; set; }
}
