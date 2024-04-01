using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Users.Queries
{
    public class GetUserByIdRequestHandler(DesicContext desicContext, ILogger<GetUserByIdRequestHandler> logger) : IRequestHandler<GetUserByIdRequest, User?>
    {
        private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
        private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<User?> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { { new("handlerType", nameof(GetUserByIdRequestHandler)) }, { new("requestUserId", request.UserId) } });
            _logger.LogTrace("Handling {requestType}: {@request}", nameof(GetUserByIdRequest), request);
            _logger.LogDebug("Handling {requestType}", nameof(GetUserByIdRequest));

            return await _desicContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        }
    }
}
