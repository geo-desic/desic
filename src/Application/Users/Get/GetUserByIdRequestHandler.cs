using Desic.Application.Common;
using Desic.Application.Common.Helpers;
using Desic.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Get;

public class GetUserByIdRequestHandler(ILogger<GetUserByIdRequestHandler> logger, IApplicationDbContext dbContext) : IRequestHandler<GetUserByIdRequest, Result<User>>
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<Result<User>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.GetEntityByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogDebug(LogEvents.UserGet, "User with id {UserId} not found", request.UserId);
            return (User?)null;
        }
        var result = new User
        {
            Id = user.Id,
            Username = user.Username,
        };
        result.MapCreatedModifiedDeleted(user);
        return result;
    }
}
