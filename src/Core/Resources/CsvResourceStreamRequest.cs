using MediatR;
using System.Text;

namespace Desic.Core.Resources;

public class CsvResourceStreamRequest<T> : IStreamRequest<T>
{
    public Type? ClassMapType { get; set; }
    public Encoding? Encoding { get; set; }
    public required string ResourceName { get; set; }
}
