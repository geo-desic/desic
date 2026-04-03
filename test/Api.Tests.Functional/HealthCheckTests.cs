using AwesomeAssertions;
using Desic.Api.Dtos.HealthChecks;
using Desic.Infrastructure.Data;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Http;

namespace Desic.Api.Tests.Functional;

public class HealthCheckTests(SeededAppDatabase testDatabase, ITestOutputHelper output) : TestWebAppDependencyTests(testDatabase, output), IClassFixture<SeededAppDatabase>
{
    private const string Healthy = nameof(Healthy);
    private const string PartialRequestUri = "/v1/healthz";

    [Fact]
    public async Task Live_ValidRequest_Status200OkAndHealthy()
    {
        // arrange
        var request = new FluentHttpRequest(HttpMethod.Get, $"{PartialRequestUri}/live");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsString(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().Be(Healthy);
    }

    [Fact]
    public async Task Ready_ValidRequest_Status200OkAndHealthy()
    {
        // arrange
        var request = new FluentHttpRequest(HttpMethod.Get, $"{PartialRequestUri}/ready");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsString(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().Be(Healthy);
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
        var request = new FluentHttpRequest(HttpMethod.Get, $"{PartialRequestUri}/report");

        // act
        var response = await HttpClient.SendAsyncAndReadResponseAsJson<HealthReport>(request: request, output: Output);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Build).Excluding(x => x.TotalDurationMilliseconds).For(x => x.Entries).Exclude(x => x.DurationMilliseconds));
    }
}
