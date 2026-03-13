using AwesomeAssertions;
using Desic.Application.Users;
using Desic.Application.Users.Create;
using Desic.Domain.Common.Entities;
using Desic.Domain.Tags;
using Desic.Domain.Users.Test;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Tests.Functional.Controllers.V1;

public class UsersControllerTests(TestDatabase testDatabase) : FunctionalTests(testDatabase), IClassFixture<TestDatabase>
{
    private readonly TimeSpan _acceptablePrecision = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task Get_UserExists_Status200OkAndUserReturned()
    {
        // arrange
        var expected = TestUsers.User01Active.ToDto(); // exists in seeded data
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/users/{expected.Id}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<User>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_UserDoesNotExist_Status404NotFound()
    {
        // arrange
        var id = new Guid("A0000000-0000-0000-0000-000000000001"); // does not exist in seeded data
        var request = new FluentHttpRequest(HttpMethod.Get, $"/v1/users/{id}");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request);

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
            Username = TestUsers.User01Active.Username, // exists in seeded data
        };
        var request = new FluentHttpRequest(HttpMethod.Post, $"/v1/users/").SetJsonContent(user);

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<ProblemDetails>(request);

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
        var response = await HttpClient.SendAsyncAndReadResponseAsString(request);

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
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<User>(request);

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
