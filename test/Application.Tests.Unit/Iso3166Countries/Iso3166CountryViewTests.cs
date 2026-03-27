using AwesomeAssertions;
using Desic.Application.Iso3166Countries;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Iso3166Countries;

public class Iso3166CountryViewTests
{
    public class Iso3166CountryViewTests001 : Iso3166CountryViewTests
    {
        [Fact]
        public void Contructor_WithIso3166CountryEntity_AllPropertiesMappedCorrectly()
        {
            // arrange
            var entity = new Domain.Iso3166Countries.Iso3166Country
            {
                Id = 1.ToGuid(),
                IsoId = 1,
                Alpha2 = "aa",
                Alpha3 = "bbb",
                Name = "ExpectedName",
            };
            var expected = new Iso3166CountryView
            {
                Id = entity.Id,
                IsoId = entity.IsoId,
                Alpha2 = entity.Alpha2,
                Alpha3 = entity.Alpha3,
                Name = entity.Name,
            };

            // act
            var result = new Iso3166CountryView(entity);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
