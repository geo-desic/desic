using AwesomeAssertions;
using Desic.Api.Tests.Functional.Common;
using Desic.Application.EntityTypes;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class EntityTypesControllerTests(TestDatabase testDatabase) : FunctionalTests(testDatabase), IClassFixture<TestDatabase>
{
    [Fact]
    public async Task List_All_Status200OkAndEntitiesReturned()
    {
        // arrange
        var expected = ExpectedResponseContent(); // all entity types should exist by db seeding
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/entitytypes");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<DeserializablePaginatedList<EntityType>>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering());
    }

    private static DeserializablePaginatedList<EntityType> ExpectedResponseContent()
    {
        List<EntityType> items = [.. Domain.EntityTypes.SystemEntityTypes.AllAsEntities().Select(x => new EntityType { Key = x.Key, Name = x.Name }).OrderBy(x => x.Name)];
        return new()
        {
            StartIndex = 0,
            TotalCount = items.Count,
            Items = items,
        };
    }
}
