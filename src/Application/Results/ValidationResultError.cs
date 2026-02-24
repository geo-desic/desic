using FluentResults;

namespace Desic.Application.Results;

internal class ValidationResultError(string message, string? propertyName, string? severity) : Error(message)
{
    public string? PropertyName { get; set; } = propertyName;
    public string? Severity { get; set; } = severity;
}
