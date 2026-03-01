namespace Desic.Application.Common.Models;

public class OptionalOnByType
{
    public OptionalBy By { get; set; } = new();
    public DateTime? On { get; set; }
}
