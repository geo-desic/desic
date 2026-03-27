using AwesomeAssertions;
using Desic.Domain.Iso3166Countries;
using Xunit.Sdk;

namespace Desic.Domain.Tests.Unit.Iso3166Countries;

public class Iso3166CountryExtensionsTests
{
    public class Iso3166CountryExtensionsTests001 : Iso3166CountryExtensionsTests
    {
        public static IEnumerable<TheoryDataRow<bool, TestItem?, TestItem?>> IsEquivalentToTheoryData()
        {
            TestItem? item1, item2;

            // true: all fields equal
            item1 = NewItem();
            item2 = NewItem();
            yield return new(true, item1, item2);

            // true: both null
            item1 = null;
            item2 = null;
            yield return new(true, item1, item2);

            // false: only item1 null
            item1 = null;
            item2 = NewItem();
            yield return new(false, item1, item2);

            // false: only item2 null
            item1 = NewItem();
            item2 = null;
            yield return new(false, item1, item2);

            // false: property does not match: IsoId
            item1 = NewItem();
            item2 = NewItem();
            item2.IsoId = 0;
            yield return new(false, item1, item2);

            // false: property does not match: Alpha2
            item1 = NewItem();
            item2 = NewItem();
            item2.Alpha2 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Alpha3
            item1 = NewItem();
            item2 = NewItem();
            item2.Alpha3 = "does-not-match";
            yield return new(false, item1, item2);

            // false: property does not match: Name
            item1 = NewItem();
            item2 = NewItem();
            item2.Name = "does-not-match";
            yield return new(false, item1, item2);
        }

        [Theory]
        [MemberData(nameof(IsEquivalentToTheoryData))]
        public void IsEquivalentTo_SpecifiedTheoryData_ExpectedResult(bool expected, TestItem? item1, TestItem? item2)
        {
            Iso3166CountryExtensions.IsEquivalentTo(item1, item2).Should().Be(expected);
        }
    }

    public class Iso3166CountryExtensionsTests002 : Iso3166CountryExtensionsTests
    {
        [Fact]
        public void UpdateFrom_ItemWithDifferentValues_UpdatesAllProperties()
        {
            // arrange
            var item = NewItem();
            var expected = new TestItem { IsoId = 2, Alpha2 = "alpha2-updated", Alpha3 = "alpha3-updated", Name = "name-updated" }; // all values different from item

            // act
            Iso3166CountryExtensions.UpdateFrom(item, expected);

            // assert
            item.IsoId.Should().Be(expected.IsoId);
            item.Alpha2.Should().Be(expected.Alpha2);
            item.Alpha3.Should().Be(expected.Alpha3);
            item.Name.Should().Be(expected.Name);
        }
    }

    private static TestItem NewItem()
    {
        return new TestItem { IsoId = 1, Alpha2 = "alpha2", Alpha3 = "alpha3", Name = "name" };
    }

    public sealed class TestItem : IIso3166CountryReferenceData, IXunitSerializable
    {
        public string Alpha2 { get; set; } = string.Empty;
        public string Alpha3 { get; set; } = string.Empty;
        public int IsoId { get; set; }
        public string Name { get; set; } = string.Empty;

        public void Deserialize(IXunitSerializationInfo info)
        {
            Alpha2 = info.GetValue<string>(nameof(Alpha2))!;
            Alpha3 = info.GetValue<string>(nameof(Alpha3))!;
            IsoId = info.GetValue<int>(nameof(IsoId))!;
            Name = info.GetValue<string>(nameof(Name))!;
        }

        public bool Equals(IIso3166CountryReferenceData? other) => throw new NotImplementedException(); // should not get called => testing extension methods directly, not this

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Alpha2), Alpha2);
            info.AddValue(nameof(Alpha3), Alpha3);
            info.AddValue(nameof(IsoId), IsoId);
            info.AddValue(nameof(Name), Name);
        }

        public void UpdateFrom(IIso3166CountryReferenceData compare) => throw new NotImplementedException(); // should not get called => testing extension methods directly, not this
    }
}
