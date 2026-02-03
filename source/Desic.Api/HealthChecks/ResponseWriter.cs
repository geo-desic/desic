using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Api.HealthChecks
{
    internal static class ResponseWriter
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions;

        static ResponseWriter()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                },
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        internal static Task Write(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var result = new Dtos.HealthChecks.HealthReport
            {
                OverallStatus = healthReport.Status,
                TotalDurationMilliseconds = (long)healthReport.TotalDuration.TotalMilliseconds,
                Entries = [.. healthReport.Entries.Select(e => new Dtos.HealthChecks.HealthReportEntry
                {
                    Data = e.Value.Data,
                    DurationMilliseconds = (long)e.Value.Duration.TotalMilliseconds,
                    Name = e.Key,
                    Status = e.Value.Status,
                    Tags = e.Value.Tags,
                })],
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(result, options: _jsonSerializerOptions));
        }
    }
}
