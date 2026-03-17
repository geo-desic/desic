using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Desic.Shared.Mediator;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        _logger.LogDebug("Handling {RequestType}", requestType);
        _logger.LogTrace("{RequestType}: {@Request}", requestType, request);
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        var responseType = typeof(TResponse).Name;
        _logger.LogDebug("Handled {RequestType} returning {ResponseType} in {HandlerTotalMilliseconds}ms", requestType, responseType, stopwatch.Elapsed.TotalMilliseconds);
        _logger.LogTrace("{ResponseType}: {@Response}", responseType, response);

        return response;
    }
}
