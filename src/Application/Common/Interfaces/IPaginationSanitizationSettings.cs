using Microsoft.Extensions.Logging;

namespace Desic.Application.Common.Interfaces;

internal interface IPaginationSanitizationSettings
{
    bool IncludeTotalCountAllowed { get; set; }
    int MaximumAllowedCount { get; set; }
    ILogger? Logger { get; set; }
    EventId LogEventId { get; set; }
}
