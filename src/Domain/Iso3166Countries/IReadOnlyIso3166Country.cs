namespace Desic.Domain.Iso3166Countries;

public interface IReadOnlyIso3166Country
{
    int IsoId { get; }
    string Alpha2 { get; }
    string Alpha3 { get; }
    string Name { get; }
}
