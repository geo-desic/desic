using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.Iso3166Countries;

public interface IReadOnlyIso3166CountryEntity : IReadOnlyGuidIdIso3166Country, IReadOnlySoftDeletableEntity
{
}
