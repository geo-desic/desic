using AwesomeAssertions;
using Desic.Domain.Iso3166Countries;

namespace Desic.Domain.Tests.Unit.Iso3166Countries;

public class Iso3166CountryHelpersTests
{
    public class Iso3166CountryHelpersTests001 : Iso3166CountryHelpersTests
    {
        public static IEnumerable<TheoryDataRow<bool, IIso3166CountryReferenceData?, IIso3166CountryReferenceData?>> IsEquivalentToTheoryData()
        {
            Iso3166Country? item1, item2;

            // true: all fields equal
            item1 = NewCountry();
            item2 = NewCountry();
            yield return new(true, item1, item2);

            // true: both null
            item1 = null;
            item2 = null;
            yield return new(true, item1, item2);

            // false: only item1 null
            item1 = null;
            item2 = NewCountry();
            yield return new(false, item1, item2);

            // false: only item2 null
            item1 = NewCountry();
            item2 = null;
            yield return new(false, item1, item2);

            // false: property does not match: IsoId
            item1 = NewCountry();
            item2 = NewCountry();
            item2.IsoId = 0;
            yield return new(false, item1, item2);

            // false: property does not match: Alpha2
            item1 = NewCountry();
            item2 = NewCountry();
            item2.Alpha2 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Alpha3
            item1 = NewCountry();
            item2 = NewCountry();
            item2.Alpha3 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Name
            item1 = NewCountry();
            item2 = NewCountry();
            item2.Name = "does-not-match";
            yield return new(false, item1, item2);
        }

        [Theory]
        [MemberData(nameof(IsEquivalentToTheoryData))]
        public void IsEquivalentTo_SpecifiedTheoryData_ExpectedResult(bool expected, IIso3166CountryReferenceData? item1, IIso3166CountryReferenceData? item2)
        {
            item1.IsEquivalentTo(item2).Should().Be(expected);
        }

        [Fact]
        public void UpdateFrom_ItemWithDifferentValues_UpdatesAllProperties()
        {
            // arrange
            var item = NewCountry();
            var expected = new Iso3166Country { IsoId = 2, Alpha2 = "alpha2-updated", Alpha3 = "alpha3-updated", Name = "name-updated" };

            // act
            item.UpdateFrom(expected);

            // assert
            item.IsoId.Should().Be(expected.IsoId);
            item.Alpha2.Should().Be(expected.Alpha2);
            item.Alpha3.Should().Be(expected.Alpha3);
            item.Name.Should().Be(expected.Name);
        }
    }

    public static Iso3166Country NewCountry()
    {
        return new Iso3166Country { IsoId = 1, Alpha2 = "alpha2", Alpha3 = "alpha3", Name = "name" };
    }
}
