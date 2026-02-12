using AwesomeAssertions;
using Desic.Api.Dtos.Users;
using Desic.Testing.Integration.Core.Db;
using Desic.Testing.Integration.Core.Http;
using Desic.Testing.Integration.Core.WebApplication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Integration;

public class UsersControllerTests : IClassFixture<DesicContextMsSqlContainer>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public UsersControllerTests(DesicContextMsSqlContainer container)
    {
        _factory = new TestWebApplicationFactory<Program>(container.ConnectionString);
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Get_UserExists_Status200OkAndUserReturned()
    {
        // arrange
        var expected = new User
        {
            Id = new Guid("00000004-0000-0000-0000-000000000001"),
            Username = "user1",
        };
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/users/{expected.Id}");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<User>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_UserDoesNotExist_Status404NotFound()
    {
        // arrange
        var id = new Guid("A0000000-0000-0000-0000-000000000001");
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/users/{id}");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        response.Content.Should().NotBeNull();
        response.Content.Status.Should().Be(404);
    }
}
