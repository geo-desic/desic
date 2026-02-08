using System.Diagnostics;

namespace Desic.Api.Middleware;

// unneccessary microsoft middleware already logs traceid and spanid and activity id is just a combination of them
public class ActivityIdLoggingMiddleware(RequestDelegate next, ILogger<ActivityIdLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ActivityIdLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var activityId = Activity.Current?.Id ?? context.TraceIdentifier;
        using (_logger.BeginScope("ActivityId:{activityId}", activityId))
        {
            await _next(context);
        }
    }
}
