using AwesomeAssertions;
using Desic.Api.Dtos;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class EntityTypesControllerTests(SeededAppDatabase testDatabase, ITestOutputHelper output) : TestWebAppDependencyTests(testDatabase, output), IClassFixture<SeededAppDatabase>
{
    private const string RequestUri = "/v1/entitytypes";

    [Fact]
    public async Task List_ValidRequestWithNonDefaultStartIndexCountAndOrderingMethod_Status200OkAndExpectedOrderedItems()
    {
        // arrange
        var count = 5;
        var includeTotalCount = true;
        var orderingMethod = new OrderingMethod<EntityTypesOrderingProperty>
        {
            OrderBy = [new OrderBy<EntityTypesOrderingProperty> { Ascending = false, Property = EntityTypesOrderingProperty.Key }],
        };
        var startIndex = 1;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = ExpectedResponseContent(count: count, includeTotalCount: includeTotalCount, startIndex: startIndex, orderingMethod: orderingMethod); // all entity types should exist by db seeding
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
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListEntityTypesResult>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering());
    }

    [Fact]
    public async Task List_ValidRequestWithFilterByKey_Status200OkAndExpectedItem()
    {
        // arrange
        var expectedItem = new EntityType(Domain.EntityTypes.SystemEntityTypes.Label.ToEntity()); // should exist by db seeding
        var key = expectedItem.Key;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = new ListEntityTypesResult
        {
            StartIndex = 0,
            TotalCount = null,
            Items = [expectedItem],
        };
        var request = new FluentHttpRequest(HttpMethod.Get, RequestUri)
            .AddQueryStringParameter(name: nameof(key), value: $"{key}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListEntityTypesResult>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected);
    }

    private static ListEntityTypesResult ExpectedResponseContent(int? count = null, bool includeTotalCount = false, int startIndex = 0, IOrderingMethod<EntityTypesOrderingProperty>? orderingMethod = null)
    {
        orderingMethod ??= OrderingMethod<EntityTypesOrderingProperty>.Default;
        var allItems = Domain.EntityTypes.SystemEntityTypes.AllAsEntities().ToList();
        var query = allItems.AsQueryable().OrderBy(orderingMethod: orderingMethod).Skip(startIndex);
        if (count.HasValue) query = query.Take(count.Value);
        List<EntityType> items = [.. query.SelectToModel()];
        return new()
        {
            StartIndex = startIndex,
            TotalCount = includeTotalCount ? allItems.Count : null,
            Items = items,
        };
    }
}
