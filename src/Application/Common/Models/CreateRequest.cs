namespace Desic.Application.Common.Models;

public class CreateRequest<T>
{
    public required T Model { get; set; }
    public bool ReturnRepresentation { get; set; }
}
