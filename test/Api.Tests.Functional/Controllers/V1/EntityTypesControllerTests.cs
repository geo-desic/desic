using AwesomeAssertions;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class EntityTypesControllerTests(SeededAppDatabase testDatabase) : TestWebAppDependencyTests(testDatabase), IClassFixture<SeededAppDatabase>
{
    [Fact]
    public async Task List_ValidRequestWithNonDefaultStartIndexCountAndOrderingMethod_Status200OkAndExpectedOrderedItems()
    {
        // arrange
        var count = 5;
        var includeTotalCount = true;
        var orderingMethod = EntityTypesOrderingMethod.KeyDesc;
        var startIndex = 1;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = ExpectedResponseContent(count: count, includeTotalCount: includeTotalCount, startIndex: startIndex, orderingMethod: orderingMethod); // all entity types should exist by db seeding
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/entitytypes")
            .AddQueryStringParameter(name: nameof(count), value: $"{count}")
            .AddQueryStringParameter(name: nameof(includeTotalCount), value: $"{includeTotalCount}")
            .AddQueryStringParameter(name: nameof(orderingMethod), value: $"{orderingMethod}")
            .AddQueryStringParameter(name: nameof(startIndex), value: $"{startIndex}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListEntityTypesResult>(request);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering());
    }

    [Fact]
    public async Task List_ValidRequestWithFilterByKey_Status200OkAndExpectedItem()
    {
        // arrange
        var expectedItem = Domain.EntityTypes.SystemEntityTypes.Label.ToEntity().ToModel(); // should exist by db seeding
        var key = expectedItem.Key;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = new ListEntityTypesResult
        {
            StartIndex = 0,
            TotalCount = null,
            Items = [expectedItem],
        };
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/entitytypes")
            .AddQueryStringParameter(name: nameof(key), value: $"{key}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListEntityTypesResult>(request);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected);
    }

    private static ListEntityTypesResult ExpectedResponseContent(int? count = null, bool includeTotalCount = false, int startIndex = 0, EntityTypesOrderingMethod? orderingMethod = null)
    {
        var allItems = Domain.EntityTypes.SystemEntityTypes.AllAsEntities().ToList();
        var query = allItems.AsQueryable().OrderBy(orderingMethod: orderingMethod).Skip(startIndex);
        if (count.HasValue) query = query.Take(count.Value);
        List<EntityType> items = [.. query.Select(x => new EntityType { Key = x.Key, Name = x.Name })];
        return new()
        {
            StartIndex = startIndex,
            TotalCount = includeTotalCount ? allItems.Count : null,
            Items = items,
        };
    }
}
