using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Users.Queries
{
    public class GetUserByUsernameRequestHandler(DesicContext desicContext, ILogger<GetUserByUsernameRequestHandler> logger) : IRequestHandler<GetUserByUsernameRequest, User?>
    {
        private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
        private readonly ILogger<GetUserByUsernameRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<User?> Handle(GetUserByUsernameRequest request, CancellationToken cancellationToken)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object?>> { { new("handlerType", nameof(GetUserByUsernameRequestHandler)) }, { new("requestUsername", request.Username) } });
            _logger.LogTrace("Handling {requestType}: {@request}", nameof(GetUserByUsernameRequest), request);
            _logger.LogDebug("Handling {requestType}", nameof(GetUserByUsernameRequest));

            return await _desicContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
        }

    }
}
