using AwesomeAssertions;
using Desic.Api.Dtos.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Api.Tests.Integration
{
    public class HealthCheckTests(CustomWebApplicationFactory<Program> factory) : IntegrationTestClassFixture(factory)
    {
        [Fact]
        public async Task Live_ValidRequest_Status200OkAndHealthy()
        {
            // act
            var response = await _client.GetAsync("/v1/healthz/live", TestContext.Current.CancellationToken);
            var contentString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            contentString.Should().Be("Healthy");
        }

        [Fact]
        public async Task Ready_ValidRequest_Status200OkAndHealthy()
        {
            // act
            var response = await _client.GetAsync("/v1/healthz/ready", TestContext.Current.CancellationToken);
            var contentString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            contentString.Should().Be("Healthy");
        }

        [Fact]
        public async Task Report_ValidRequest_Status200OkAndHealthy()
        {
            // arrange
            var healthy = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
            var expectedEntries = new HealthReportEntry[]
            {
                new() { Name = "Alive", Status = healthy, Tags = [], Data = new Dictionary<string, object>() },
                new() { Name = "DesicContext", Status = healthy, Tags = ["ready"], Data = new Dictionary<string, object>() },
                new() { Name = "Startup", Status = healthy, Tags = ["ready"], Data = new Dictionary<string, object>() },
            };

            // act
            var response = await _client.GetAsync("/v1/healthz/report", TestContext.Current.CancellationToken);
            var contentString = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            var result = JsonDeserialize<HealthReport>(contentString);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.OverallStatus.Should().Be(healthy);
            result.Entries.Should().BeEquivalentTo(expectedEntries, opt => opt.Excluding(x => x.DurationMilliseconds));
        }

        private static T? JsonDeserialize<T>(string value)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() },
                    PropertyNameCaseInsensitive = true,
                };
                return JsonSerializer.Deserialize<T>(value, options);
            }
            catch { }
            return default;
        }
    }
}
