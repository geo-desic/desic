using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desic.Api.HealthChecks;

internal static class ResponseWriter
{
    private static readonly Assembly _executingAssembly = Assembly.GetExecutingAssembly();
    private static readonly List<AssemblyMetadataAttribute> _metadataAttributes = [.. Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>()];

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters =
            {
                new JsonStringEnumConverter(),
            },
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    internal static Task Write(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var result = new Dtos.HealthChecks.HealthReport
        {
            Build = new Dtos.HealthChecks.BuildInformation
            {
                CommitSha = GetAssemblyMetadataValue($"{nameof(Dtos.HealthChecks.HealthReport.Build)}{nameof(Dtos.HealthChecks.BuildInformation.CommitSha)}"),
                CreatedOn = GetAssemblyMetadataValueAsDateTime($"{nameof(Dtos.HealthChecks.HealthReport.Build)}{nameof(Dtos.HealthChecks.BuildInformation.CreatedOn)}"),
                RunAttempt = GetAssemblyMetadataValueAsLong($"{nameof(Dtos.HealthChecks.HealthReport.Build)}{nameof(Dtos.HealthChecks.BuildInformation.RunAttempt)}"),
                RunId = GetAssemblyMetadataValueAsLong($"{nameof(Dtos.HealthChecks.HealthReport.Build)}{nameof(Dtos.HealthChecks.BuildInformation.RunId)}"),
                RunNumber = GetAssemblyMetadataValueAsLong($"{nameof(Dtos.HealthChecks.HealthReport.Build)}{nameof(Dtos.HealthChecks.BuildInformation.RunNumber)}"),
                Version = _executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            },
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

    private static string? GetAssemblyMetadataValue(string key)
    {
        return _metadataAttributes.FirstOrDefault(a => a.Key == key)?.Value;
    }

    private static DateTime? GetAssemblyMetadataValueAsDateTime(string key)
    {
        if (DateTime.TryParse(GetAssemblyMetadataValue(key), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result)) return result;
        return null;
    }

    private static long? GetAssemblyMetadataValueAsLong(string key)
    {
        if (long.TryParse(GetAssemblyMetadataValue(key), out var result)) return result;
        return null;
    }
}
