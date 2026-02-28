using AwesomeAssertions;
using Desic.Domain.Iso3166Countries;

namespace Desic.Domain.Tests.Unit.Iso3166Countries;

public class Iso3166CountryHelpersTests
{
    public class Iso3166CountryHelpersTests001 : Iso3166CountryHelpersTests
    {
        public static IEnumerable<TheoryDataRow<bool, IIso3166CountryReferenceData?, IIso3166CountryReferenceData?>> IsEquivalentToTheoryData()
        {
            IIso3166CountryReferenceData? item1, item2;

            // true: all fields equal
            item1 = NewCountryReferenceData();
            item2 = NewCountryReferenceData();
            yield return new(true, item1, item2);

            // true: both null
            item1 = null;
            item2 = null;
            yield return new(true, item1, item2);

            // false: only item1 null
            item1 = null;
            item2 = NewCountryReferenceData();
            yield return new(false, item1, item2);

            // false: only item2 null
            item1 = NewCountryReferenceData();
            item2 = null;
            yield return new(false, item1, item2);

            // false: property does not match: IsoId
            item1 = NewCountryReferenceData();
            item2 = NewCountryReferenceData();
            item2.IsoId = 0;
            yield return new(false, item1, item2);

            // false: property does not match: Alpha2
            item1 = NewCountryReferenceData();
            item2 = NewCountryReferenceData();
            item2.Alpha2 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Alpha3
            item1 = NewCountryReferenceData();
            item2 = NewCountryReferenceData();
            item2.Alpha3 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Name
            item1 = NewCountryReferenceData();
            item2 = NewCountryReferenceData();
            item2.Name = "does-not-match";
            yield return new(false, item1, item2);
        }

        [Theory]
        [MemberData(nameof(IsEquivalentToTheoryData))]
        public void IsEquivalentTo_SpecifiedTheoryData_ExpectedResult(bool expected, IIso3166CountryReferenceData? item1, IIso3166CountryReferenceData? item2)
        {
            Iso3166CountryHelpers.IsEquivalentTo(item1, item2).Should().Be(expected);
        }
    }

    public class Iso3166CountryHelpersTests002 : Iso3166CountryHelpersTests
    {
        [Fact]
        public void UpdateFrom_ItemWithDifferentValues_UpdatesAllProperties()
        {
            // arrange
            var item = NewCountryReferenceData();
            var expected = new TestCountryReferenceData { IsoId = 2, Alpha2 = "alpha2-updated", Alpha3 = "alpha3-updated", Name = "name-updated" }; // all values different from item

            // act
            Iso3166CountryHelpers.UpdateFrom(item, expected);

            // assert
            item.IsoId.Should().Be(expected.IsoId);
            item.Alpha2.Should().Be(expected.Alpha2);
            item.Alpha3.Should().Be(expected.Alpha3);
            item.Name.Should().Be(expected.Name);
        }
    }

    private static TestCountryReferenceData NewCountryReferenceData()
    {
        return new TestCountryReferenceData { IsoId = 1, Alpha2 = "alpha2", Alpha3 = "alpha3", Name = "name" };
    }

    private class TestCountryReferenceData : IIso3166CountryReferenceData
    {
        public int IsoId { get; set; }
        public string Alpha2 { get; set; } = string.Empty;
        public string Alpha3 { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public bool Equals(IIso3166CountryReferenceData? other) => throw new NotImplementedException(); // should not get called => testing extension methods directly, not this

        public void UpdateFrom(IIso3166CountryReferenceData compare) => throw new NotImplementedException(); // should not get called => testing extension methods directly, not this
    }
}
