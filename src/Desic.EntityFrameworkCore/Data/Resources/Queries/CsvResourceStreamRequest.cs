using MediatR;
using System.Text;

namespace Desic.EntityFrameworkCore.Data.Resources.Queries;

public class CsvResourceStreamRequest<T> : IStreamRequest<T>
{
    public Type? ClassMapType { get; set; }
    public Encoding? Encoding { get; set; }
    public required string ResourceName { get; set; }
}
