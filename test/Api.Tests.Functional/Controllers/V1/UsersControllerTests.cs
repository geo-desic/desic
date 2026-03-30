using AwesomeAssertions;
using Desic.Api.Common;
using Desic.Application.Users;
using Desic.Application.Users.Create;
using Desic.Domain.Common.Interfaces;
using Desic.Domain.Labels;
using Desic.Domain.Users.Test;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class UsersControllerTests(SeededAppDatabase testDatabase, ITestOutputHelper output) : TestWebAppDependencyTests(testDatabase, output), IClassFixture<SeededAppDatabase>
{
    private readonly TimeSpan _acceptablePrecision = TimeSpan.FromSeconds(1);
    private const string RequestUri = "/v1/users";

    [Fact]
    public async Task Get_UserExists_Status200OkAndUserReturned()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expected = TestUsers.User01Active.ToModel(); // exists in seeded data
        var request = new FluentHttpRequest(HttpMethod.Get, $"{RequestUri}/{expected.Id}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<User>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_UserDoesNotExist_Status404NotFound()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.NotFound;
        var id = new Guid("A0000000-0000-0000-0000-000000000001"); // does not exist in seeded data
        var request = new FluentHttpRequest(HttpMethod.Get, $"{RequestUri}/{id}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Content.Should().NotBeNull();
        response.Content.Status.Should().Be((int)expectedStatusCode);
    }

    [Fact]
    public async Task Create_InvalidRequestUsernameInvalid_Status400()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var user = new CreateUser
        {
            Username = "invalid username", // contains a space character
        };
        var request = new FluentHttpRequest(HttpMethod.Post, RequestUri).SetJsonContent(user);

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Message.Headers.TryGetValues(Headers.Keys.EntityId, out _).Should().BeFalse();
        response.Content.Should().NotBeNull();
        response.Content.Status.Should().Be((int)expectedStatusCode);
    }

    [Fact]
    public async Task Create_InvalidRequestUsernameExists_Status400()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var user = new CreateUser
        {
            Username = TestUsers.User01Active.Username, // exists in seeded data
        };
        var request = new FluentHttpRequest(HttpMethod.Post, RequestUri).SetJsonContent(user);

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Message.Headers.TryGetValues(Headers.Keys.EntityId, out _).Should().BeFalse();
        response.Content.Should().NotBeNull();
        response.Content.Status.Should().Be((int)expectedStatusCode);
    }

    [Fact]
    public async Task Create_ValidRequestNoPreferHeader_Status204WithEntityIdHeader()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.NoContent;
        var user = new CreateUser
        {
            Username = "username-does-not-exist-1",
        };
        var request = new FluentHttpRequest(HttpMethod.Post, RequestUri).SetJsonContent(user);

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsString(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Message.Headers.TryGetValues(Headers.Keys.EntityId, out var values).Should().BeTrue();
        values.Should().HaveCount(1);
        Guid.TryParse(values.First(), out _).Should().BeTrue();
        response.Content.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Create_ValidRequestWithPreferRepresentationHeader_Status201AndUserReturned()
    {
        // arrange
        var expectedStatusCode = System.Net.HttpStatusCode.Created;
        var user = new CreateUser
        {
            Username = "username-does-not-exist-2",
        };
        var expected = NewUser(username: "username-does-not-exist-2");
        var request = new FluentHttpRequest(HttpMethod.Post, RequestUri).SetJsonContent(user).AddHeader(Headers.Keys.Prefer, Headers.Values.PreferRepresentation);

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<User>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(expectedStatusCode);
        response.Message.Headers.TryGetValues(Headers.Keys.EntityId, out var values).Should().BeTrue();
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
        by ??= SystemLabels.System;
        var byNamed = by as IReadOnlyNameable;
        return new User
        {
            Id = id ?? Guid.Empty,
            Username = username ?? "username",
            Created = new()
            {
                By = new()
                {
                    Id = by.Id,
                    Name = byNamed?.Name,
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
                    Name = byNamed?.Name,
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
                    Name = null,
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
