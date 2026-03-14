namespace Desic.Application.Common.Models;

public class CreateResult<T> where T : class
{
    public Guid Id { get; set; }
    public T? Model { get; set; }
}
