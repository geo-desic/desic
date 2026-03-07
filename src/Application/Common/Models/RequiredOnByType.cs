namespace Desic.Application.Common.Models;

public class RequiredOnByType
{
    public RequiredBy By { get; set; } = new();
    public DateTime On { get; set; }
}
