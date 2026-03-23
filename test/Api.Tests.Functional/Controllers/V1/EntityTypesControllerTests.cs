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
        var startIndex = 1;
        var count = 5;
        var orderingMethod = EntityTypesOrderingMethod.KeyDesc;
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = ExpectedResponseContent(startIndex: startIndex, count: count, orderingMethod: orderingMethod); // all entity types should exist by db seeding
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/entitytypes")
            .AddQueryStringParameter(name: nameof(startIndex), value: $"{startIndex}")
            .AddQueryStringParameter(name: nameof(count), value: $"{count}")
            .AddQueryStringParameter(name: nameof(orderingMethod), value: $"{orderingMethod}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ListEntityTypesResult>(request);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering());
    }

    private static ListEntityTypesResult ExpectedResponseContent(int? startIndex = null, int? count = null, EntityTypesOrderingMethod? orderingMethod = null)
    {
        startIndex ??= 0;
        var allItems = Domain.EntityTypes.SystemEntityTypes.AllAsEntities().Select(x => new EntityType { Key = x.Key, Name = x.Name }).ToList();
        var query = allItems.AsQueryable().OrderBy(orderingMethod: orderingMethod).Skip(startIndex.Value);
        if (count.HasValue) query = query.Take(count.Value);
        List<EntityType> items = [.. query];
        return new()
        {
            StartIndex = startIndex.Value,
            TotalCount = allItems.Count,
            Items = items,
        };
    }
}
