using Desic.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Common.Extensions;

internal static class PaginationExtensions
{
    public static void Sanitize(this IPagination source, IPaginationSanitizationSettings settings)
    {
        if (source.Count < 0)
        {
            settings.Logger?.LogWarning(settings.LogEventId, $"Negative {nameof(IPagination.Count)} is not supported, defaulting to 0");
            source.Count = 0;
        }
        if (source.Count > settings.MaximumAllowedCount)
        {
            settings.Logger?.LogInformation(settings.LogEventId, "Requested count is greater than maximum allowed, capping at the maximum: {RequestCount} > {MaximumAllowedCount}", source.Count, settings.MaximumAllowedCount);
            source.Count = settings.MaximumAllowedCount;
        }
        if (source.StartIndex < 0)
        {
            settings.Logger?.LogWarning(settings.LogEventId, $"Negative {nameof(IPagination.StartIndex)} is not supported, defaulting to 0");
            source.StartIndex = 0;
        }
        if (!settings.IncludeTotalCountAllowed && source.IncludeTotalCount)
        {
            settings.Logger?.LogInformation(settings.LogEventId, "Including total count is not allowed for this type of request, switching to false");
            source.IncludeTotalCount = false;
        }
    }
}
