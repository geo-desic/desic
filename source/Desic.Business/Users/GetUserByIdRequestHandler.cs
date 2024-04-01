using Desic.Business.Users.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Business.Users
{
    public class GetUserByIdRequestHandler(ILogger<GetUserByIdRequestHandler> logger, IMediator mediator) : IRequestHandler<GetUserByIdRequest, User?>
    {
        private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException();

        public async Task<User?> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { { new("handlerType", nameof(GetUserByIdRequestHandler)) }, { new("requestUserId", request.UserId) } });
            _logger.LogTrace("Handling {requestType}: {@request}", nameof(GetUserByIdRequest), request);
            _logger.LogDebug("Handling {requestType}", nameof(GetUserByIdRequest));

            var query = new EntityFrameworkCore.Users.Queries.GetUserByIdRequest { UserId = request.UserId };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(EntityFrameworkCore.Users.Queries.GetUserByIdRequest), query);
            _logger.LogDebug("Invoking {requestType}", nameof(EntityFrameworkCore.Users.Queries.GetUserByIdRequest));
            var user = await _mediator.Send(query, cancellationToken);

            if (user == null)
            {
                _logger.LogDebug("User with id {id} not found", request.UserId);
                return null;
            }
            return new User
            {
                Id = user.Id,
                Username = user.Username,
            };
        }
    }
}
