using Desic.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Common.Infrastructure;

internal class PaginationSanitizationSettings : IPaginationSanitizationSettings
{
    public bool IncludeTotalCountAllowed { get; set; } = ListRequests.DefaultIncludeTotalCountAllowed;
    public int MaximumAllowedCount { get; set; } = ListRequests.DefaultMaximumAllowedCount;
    public ILogger? Logger { get; set; }
    public EventId LogEventId { get; set; }
}
