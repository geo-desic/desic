using Desic.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Common.Extensions;

internal static class ListRequestExtensions
{
    public static void Sanitize(this IListRequest request, IListRequestSanitizationSettings settings)
    {
        if (request.StartIndex < 0)
        {
            settings.Logger?.LogWarning(settings.LogEventId, $"Negative {nameof(IListRequest.StartIndex)} is not supported, defaulting to 0");
            request.StartIndex = 0;
        }
        if (request.Count < 0)
        {
            settings.Logger?.LogWarning(settings.LogEventId, $"Negative {nameof(IListRequest.Count)} is not supported, defaulting to 0");
            request.Count = 0;
        }
        if (request.Count > settings.MaximumAllowedCount)
        {
            settings.Logger?.LogInformation(settings.LogEventId, "Requested count is greater than maximum allowed, capping at the maximum: {RequestCount} > {MaximumAllowedCount}", request.Count, settings.MaximumAllowedCount);
            request.Count = settings.MaximumAllowedCount;
        }
    }
}
