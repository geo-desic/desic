using AwesomeAssertions;
using Desic.Api.Dtos;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class Iso3166CountriesControllerTests(SeededAppDatabase testDatabase, ITestOutputHelper output) : TestWebAppDependencyTests(testDatabase, output), IClassFixture<SeededAppDatabase>
{
    private const string RequestUri = "/v1/iso3166countries";

    [Fact]
    public async Task List_ValidRequestWithNonDefaultPaginationAndOrderingMethod_Status200OkAndExpectedOrderedItems()
    {
        // arrange
        var count = 2;
        var includeTotalCount = false;
        var orderingMethod = new OrderingMethod<Iso3166CountriesOrderingProperty>
        {
            OrderBy = [new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = false, Property = Iso3166CountriesOrderingProperty.Alpha2 }],
        };
        var startIndex = 1;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expectedItems = new List<Iso3166CountryView>
        {
            // these should exist by seeding; see Infrastructure/Data/Iso3166Countries/iso-3166-countries.csv
            // note the file isn't currently sorted by alpha2, so can't simply trust records at the end of the file
            // since startIndex == 1 the first will be skipped: Zimbabwe
            new() { Alpha2 = "zm", Alpha3 = "zmb", IsoId = 894, Name = "Zambia" },
            new() { Alpha2 = "za", Alpha3 = "zaf", IsoId = 710, Name = "South Africa" },
        };
        var expected = new ListIso3166CountriesResult
        {
            StartIndex = startIndex,
            TotalCount = null,
            Items = [.. expectedItems],
        };
        var request = new FluentHttpRequest(HttpMethod.Get, RequestUri)
            .AddQueryStringParameter(name: nameof(count), value: $"{count}")
            .AddQueryStringParameter(name: nameof(includeTotalCount), value: $"{includeTotalCount}")
            .AddQueryStringParameter(name: nameof(startIndex), value: $"{startIndex}");
        foreach (var item in orderingMethod.OrderBy)
        {
            request.AddQueryStringParameter(name: nameof(OrderingMethodFromQuery<>.OrderBy), value: $"{item.Property}");
            request.AddQueryStringParameter(name: nameof(OrderingMethodFromQuery<>.Asc), value: $"{item.Ascending}");
        }

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListIso3166CountriesResult>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering().For(x => x.Items).Exclude(x => x.Id));
    }

    [Fact]
    public async Task List_ValidRequestWithFilter_Status200OkAndExpecteItem()
    {
        // arrange
        var alpha3 = "usa";
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expectedItem = new Iso3166CountryView { Alpha2 = "us", Alpha3 = "usa", IsoId = 840, Name = "United States of America" };
        var expected = new ListIso3166CountriesResult
        {
            StartIndex = 0,
            TotalCount = null,
            Items = [expectedItem],
        };
        var request = new FluentHttpRequest(HttpMethod.Get, RequestUri)
            .AddQueryStringParameter(name: nameof(alpha3), value: $"{alpha3}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListIso3166CountriesResult>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.For(x => x.Items).Exclude(x => x.Id));
    }
}
