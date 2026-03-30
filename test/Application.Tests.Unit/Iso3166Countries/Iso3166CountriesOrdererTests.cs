using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Iso3166Countries;

// note: most functionality is already tested in the base class, so testing only a few things here such as all properties are mapped properly
public class Iso3166CountriesOrdererTests
{
    public class Iso3166CountriesOrdererTests001 : Iso3166CountriesOrdererTests
    {
        [Fact]
        public void Constructor_ApplyOrderingMethodCalledOnConstructedObjectForAllPossibleEnumValues_AllEnumValuesAreMapped()
        {
            // arrange
            var orderer = new Iso3166CountriesOrderer();
            var expected = GetItems().ToList();

            foreach (var value in Enum.GetValues<Iso3166CountriesOrderingProperty>())
            {
                var orderingMethod = new OrderingMethod<Iso3166CountriesOrderingProperty> { OrderBy = [new() { Property = value }] };

                // act ====> should not throw any exceptions
                var result = orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod).ToList();

                // assert
                result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }
    }

    private static IEnumerable<Domain.Iso3166Countries.Iso3166Country> GetItems()
    {
        yield return new() { Alpha2 = "aa", Alpha3 = "aaa", Id = 1.ToGuid(), IsoId = 1, Name = "A" };
    }
}
