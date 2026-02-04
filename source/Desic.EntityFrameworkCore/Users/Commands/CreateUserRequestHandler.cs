using Desic.EntityFrameworkCore.Data;
using Desic.EntityFrameworkCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Users.Commands
{
    public class CreateUserRequestHandler(DesicContext desicContext, ILogger<CreateUserRequestHandler> logger) : IRequestHandler<CreateUserRequest, Guid>
    {
        private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
        private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { { new("handlerType", nameof(CreateUserRequestHandler)) }, { new("requestUsername", request.User.Username) } });
            _logger.LogTrace("Handling {requestType}: {@request}", nameof(CreateUserRequest), request);
            _logger.LogDebug("Handling {requestType}", nameof(CreateUserRequest));

            // change later to user who created this user
            var entityTypeTag = EntityTypes.Get(Enums.EntityType.Tag);
            var tagSystem = Tags.Get(Enums.SystemTag.System);
            var now = DateTime.UtcNow;

            var user = request.User;
            user.Id = Guid.NewGuid();
            user.CreatedById = tagSystem.Id;
            user.CreatedByTypeId = entityTypeTag.Id;
            user.CreatedOn = now;
            user.ModifiedById = tagSystem.Id;
            user.ModifiedByTypeId = entityTypeTag.Id;
            user.ModifiedOn = now;
            user.IsActive = true;
            user.IsHidden = false;

            await _desicContext.Users.AddAsync(user, cancellationToken);
            await _desicContext.SaveChangesAsync(cancellationToken);
            _desicContext.Entry(user).State = EntityState.Detached;

            return user.Id;
        }

    }
}
