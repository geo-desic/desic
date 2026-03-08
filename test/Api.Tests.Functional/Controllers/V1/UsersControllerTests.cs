using AwesomeAssertions;
using Desic.Application.Users;
using Desic.Application.Users.Create;
using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;
using Desic.Testing.Integration.WebApplication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class UsersControllerTests : IClassFixture<TestDatabaseBasedOnConfig>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private readonly TimeSpan _acceptablePrecision = TimeSpan.FromSeconds(1);

    public UsersControllerTests(TestDatabaseBasedOnConfig testDatabase)
    {
        _factory = new TestWebApplicationFactory<Program>(testDatabase.GetConnectionString(), testDatabase.DbProvider);
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Get_UserExists_Status200OkAndUserReturned()
    {
        // arrange
        var expected = NewUser(id: new Guid("00000004-0000-0000-0000-000000000001"), username: "user-1", by: SystemTags.System); // this should exist in the db by test user seeding
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/users/{expected.Id}");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<User>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(expected, x => x.Excluding(x => x.Created.On).Excluding(x => x.Modified.On).Excluding(x => x.Deleted.On));
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

    [Fact]
    public async Task Create_InvalidRequestUsernameExists_Status400()
    {
        // arrange
        var user = new UserCreate
        {
            Username = "user-1", // exists, part of seeded data
        };
        var request = new FluentHttpRequest(HttpMethod.Post, $"/v1/users/").SetJsonContent(user);

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Message.Headers.TryGetValues("Entity-Id", out _).Should().BeFalse();
        response.Content.Should().NotBeNull();
        response.Content.Status.Should().Be(400);
    }

    [Fact]
    public async Task Create_ValidRequestNoPreferHeader_Status204WithEntityIdHeader()
    {
        // arrange
        var user = new UserCreate
        {
            Username = "username-does-not-exist-1",
        };
        var request = new FluentHttpRequest(HttpMethod.Post, $"/v1/users/").SetJsonContent(user);

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsString(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        response.Message.Headers.TryGetValues("Entity-Id", out var values).Should().BeTrue();
        values.Should().HaveCount(1);
        Guid.TryParse(values.First(), out _).Should().BeTrue();
        response.Content.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Create_ValidRequestWithPreferRepresentationHeader_Status201AndUserReturned()
    {
        // arrange
        var user = new UserCreate
        {
            Username = "username-does-not-exist-2",
        };
        var expected = NewUser(username: "username-does-not-exist-2");
        var request = new FluentHttpRequest(HttpMethod.Post, $"/v1/users/").SetJsonContent(user).AddHeader("Prefer", "return=representation");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<User>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        response.Message.Headers.TryGetValues("Entity-Id", out var values).Should().BeTrue();
        values.Should().HaveCount(1);
        Guid.TryParse(values.First(), out _).Should().BeTrue();
        response.Content.Should().BeEquivalentTo(expected, x => x
            .Excluding(x => x.Id)
            .Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, _acceptablePrecision)).WhenTypeIs<DateTime>()
            .Using<DateTime?>(x => x.Subject.GetValueOrDefault().Should().BeCloseTo(x.Expectation.GetValueOrDefault(), _acceptablePrecision)).WhenTypeIs<DateTime?>());
    }

    private static User NewUser(Guid? id = null, string? username = null, DateTime? on = null, IReadOnlyMinimalEntity? by = null)
    {
        on ??= DateTime.UtcNow;
        by ??= SystemTags.System;
        return new User
        {
            Id = id ?? Guid.Empty,
            Username = username ?? "username",
            Created = new()
            {
                By = new()
                {
                    Id = by.Id,
                    Type = new()
                    {
                        Key = by.SystemEntityType.Key,
                        Name = by.SystemEntityType.Name,
                    }
                },
                On = on.Value,
            },
            Modified = new()
            {
                By = new()
                {
                    Id = by.Id,
                    Type = new()
                    {
                        Key = by.SystemEntityType.Key,
                        Name = by.SystemEntityType.Name,
                    }
                },
                On = on.Value,
            },
            Deleted = new()
            {
                By = new()
                {
                    Id = null,
                    Type = new()
                    {
                        Key = null,
                    }
                },
                On = null,
            },
        };
    }
}
