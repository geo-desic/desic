using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Desic.Core.Mediator.Entities
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest).Name;
            _logger.LogDebug("Handling {requestType}", requestType);
            _logger.LogTrace("{requestType}: {@request}", requestType, request);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var response = await next(cancellationToken);
            stopwatch.Stop();

            var responseType = typeof(TResponse).Name;
            _logger.LogDebug("Handled {requestType} returning {responseType} in {handlerTotalMilliseconds}ms", requestType, responseType, stopwatch.Elapsed.TotalMilliseconds);
            _logger.LogTrace("{responseType}: {@response}", responseType, response);

            return response;
        }
    }
}
