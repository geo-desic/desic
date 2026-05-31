using Desic.Application.Common;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;
using DispatchR.Abstractions.Send;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Get;

public sealed class GetUserByIdRequestHandler(ILogger<GetUserByIdRequestHandler> logger, IApplicationDbContext dbContext) : IRequestHandler<GetUserByIdRequest, Task<Result<User>>>
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private const int LogEventId = LogEvents.GetUser;

    public async Task<Result<User>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var result = (await _dbContext.Users.GetEntityByIdAsync(id: request.Id, cancellationToken: cancellationToken))?.ToModel();
        if (result == null) _logger.LogDebug(LogEventId, "User with id {UserId} not found", request.Id);
        return result;
    }
}
