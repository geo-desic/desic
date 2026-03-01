namespace Desic.Application.Common.Models;

public class RequiredBy
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public RequiredByType Type { get; set; } = new();
}
