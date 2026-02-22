using Desic.Data.EntityTypes;
using Desic.Data.Tags;
using Desic.Data.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Data.Handlers.Users;

public class CreateUserRequestHandler(DesicContext desicContext, ILogger<CreateUserRequestHandler> logger) : IRequestHandler<CreateUserRequest, Guid>
{
    private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
    private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // change later to user who created this user
        var entityTypeTag = SystemEntityTypes.Get(SystemEntityType.Tag);
        var tagSystem = SystemTags.Get(SystemTag.System);
        var now = DateTime.UtcNow;

        var user = request.User;
        user.Id = Guid.CreateVersion7();
        user.CreatedById = tagSystem.Id;
        user.CreatedByTypeId = entityTypeTag.Id;
        user.CreatedOn = now;
        user.ModifiedById = tagSystem.Id;
        user.ModifiedByTypeId = entityTypeTag.Id;
        user.ModifiedOn = now;
        user.IsDeleted = false;
        user.DeletedOn = null;
        user.DeletedById = null;
        user.DeletedByTypeId = null;
        user.IsActive = true;

        await _desicContext.Users.AddAsync(user, cancellationToken);
        await _desicContext.SaveChangesAsync(cancellationToken);
        _desicContext.Entry(user).State = EntityState.Detached;

        return user.Id;
    }

}
