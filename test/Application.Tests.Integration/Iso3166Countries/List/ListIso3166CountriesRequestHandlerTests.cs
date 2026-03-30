using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using Desic.Testing.Integration.Db;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Application.Tests.Integration.Iso3166Countries.List;

public class ListIso3166CountriesRequestHandlerTests(SeededAppDatabase testDatabase) : TestHostDependencyTests(testDatabase), IClassFixture<SeededAppDatabase>
{
    [Fact]
    public async Task ListIso3166CountriesRequestSend_ValidRequestWithCount1_1EntityTypeReturned()
    {
        // arrange
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListIso3166CountriesRequest
        {
            Pagination = new Pagination
            {
                Count = 1,
                IncludeTotalCount = true,
                StartIndex = 0,
            },
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count.Should().Be(1);
        result.Value.TotalCount.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task ListIso3166CountriesRequestSend_ValidRequestWithNonDefaultStartIndexCountAndOrderingMethod_ExpectedResultsOrderedCorrectly()
    {
        // arrange
        var count = 2;
        var startIndex = 1;
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
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListIso3166CountriesRequest
        {
            Pagination = new Pagination
            {
                Count = count,
                IncludeTotalCount = false,
                StartIndex = startIndex,
            },
            OrderingMethod = new OrderingMethod<Iso3166CountriesOrderingProperty>
            {
                OrderBy =
                [
                    new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = false, Property = Iso3166CountriesOrderingProperty.Alpha2 }
                ],
            },
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrderingFor(x => x.Items).For(x => x.Items).Exclude(x => x.Id));
    }

    [Fact]
    public async Task ListIso3166CountriesRequestSend_ValidRequestWithFilter_ExpectedResult()
    {
        // arrange
        var expectedItem = new Iso3166CountryView { Alpha2 = "us", Alpha3 = "usa", IsoId = 840, Name = "United States of America" };
        var expected = new ListIso3166CountriesResult
        {
            StartIndex = 0,
            TotalCount = null,
            Items = [expectedItem],
        };
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var request = new ListIso3166CountriesRequest
        {
            Pagination = new Pagination
            {
                IncludeTotalCount = false,
            },
            Filter = new() { Alpha3 = expectedItem.Alpha3 },
        };

        // act
        var result = await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expected, opt => opt.For(x => x.Items).Exclude(x => x.Id));
    }
}
