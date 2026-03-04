using AwesomeAssertions;
using Desic.Api.Dtos.HealthChecks;
using Desic.Infrastructure.Data;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;
using Desic.Testing.Integration.WebApplication;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Desic.Api.Tests.Functional;

public class HealthCheckTests : IClassFixture<DbFixture>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public HealthCheckTests(DbFixture dbFixture)
    {
        _factory = new TestWebApplicationFactory<Program>(dbFixture.ConnectionStringApp);
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task Live_ValidRequest_Status200OkAndHealthy()
    {
        // arrange
        var request = new FluentHttpRequest(HttpMethod.Get, "/v1/healthz/live");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsString(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().Be("Healthy");
    }

    [Fact]
    public async Task Ready_ValidRequest_Status200OkAndHealthy()
    {
        // arrange
        var request = new FluentHttpRequest(HttpMethod.Get, "/v1/healthz/ready");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsString(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().Be("Healthy");
    }

    [Fact]
    public async Task Report_ValidRequest_Status200OkAndHealthy()
    {
        // arrange
        var healthy = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
        var expected = new HealthReport
        {
            OverallStatus = healthy,
            Entries =
            [
                new() { Name = "self", Status = healthy, Tags = ["live"], Data = new Dictionary<string, object>() },
                new() { Name = "Alive", Status = healthy, Tags = ["live"], Data = new Dictionary<string, object>() },
                new() { Name = nameof(ApplicationDbContext), Status = healthy, Tags = ["ready"], Data = new Dictionary<string, object>() },
                new() { Name = "Startup", Status = healthy, Tags = ["ready"], Data = new Dictionary<string, object>() },
            ]
        };
        var request = new FluentHttpRequest(HttpMethod.Get, "/v1/healthz/report");

        // act
        var response = await _httpClient.SendAsyncAndReadResponseAsJson<HealthReport>(request);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.TotalDurationMilliseconds).For(x => x.Entries).Exclude(x => x.DurationMilliseconds));
    }
}
