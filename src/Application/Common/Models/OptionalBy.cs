namespace Desic.Application.Common.Models;

public class OptionalBy
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public OptionalByType Type { get; set; } = new();
}
