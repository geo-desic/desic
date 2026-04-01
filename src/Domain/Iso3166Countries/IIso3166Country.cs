namespace Desic.Domain.Iso3166Countries;

public interface IIso3166Country : IReadOnlyIso3166Country
{
    new int IsoId { get; set; }
    new string Alpha2 { get; set; }
    new string Alpha3 { get; set; }
    new string Name { get; set; }
}
