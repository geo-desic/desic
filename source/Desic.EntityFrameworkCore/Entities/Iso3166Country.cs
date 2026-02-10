namespace Desic.EntityFrameworkCore.Entities;

public class Iso3166Country
{
    public required int IsoId { get; set; }
    public required string Alpha2 { get; set; }
    public required string Alpha3 { get; set; }
    public required string Name { get; set; }
}
