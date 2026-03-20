using AwesomeAssertions;
using Desic.Api.Tests.Functional.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class EntityTypesControllerTests(SeededAppDatabase testDatabase) : TestWebAppDependencyTests(testDatabase), IClassFixture<SeededAppDatabase>
{
    [Fact]
    public async Task List_All_Status200OkAndEntitiesReturned()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = ExpectedResponseContent(); // all entity types should exist by db seeding
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/entitytypes");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<DeserializableListResult<EntityType>>(request);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Should().BeEquivalentTo(expected, x => x.WithStrictOrdering());
    }

    private static DeserializableListResult<EntityType> ExpectedResponseContent()
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
