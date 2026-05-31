using DispatchR.Abstractions.Send;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Desic.Shared.Mediator;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, Task<TResponse>>
    where TRequest : class, IRequest<TRequest, Task<TResponse>>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public required IRequestHandler<TRequest, Task<TResponse>> NextPipeline { get; set; }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        _logger.LogDebug("Handling {RequestType}", requestType);
        _logger.LogTrace("{RequestType}: {@Request}", requestType, request);
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var response = await NextPipeline.Handle(request, cancellationToken);
        stopwatch.Stop();

        var responseType = typeof(TResponse).Name;
        _logger.LogDebug("Handled {RequestType} returning {ResponseType} in {HandlerTotalMilliseconds}ms", requestType, responseType, stopwatch.Elapsed.TotalMilliseconds);
        _logger.LogTrace("{ResponseType}: {@Response}", responseType, response);

        return response;
    }
}
