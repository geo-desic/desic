namespace Desic.Application.Common;

public class CreateResult<T> where T : class
{
    public Guid Id { get; set; }
    public T? Entity { get; set; }
}
