using Microsoft.Extensions.Logging;

namespace Desic.Application.Common.Interfaces;

internal interface IListRequestSanitizationSettings
{
    int MaximumAllowedCount { get; set; }
    ILogger? Logger { get; set; }
    EventId LogEventId { get; set; }
}
