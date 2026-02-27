namespace Desic.Application.Common;

public class Error(string message)
{
    public string Message { get; } = message;
}
