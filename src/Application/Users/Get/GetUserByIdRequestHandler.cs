using Desic.Application.Common;
using Desic.Application.Common.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Get;

public class GetUserByIdRequestHandler(ILogger<GetUserByIdRequestHandler> logger, IDesicContext desicContext) : IRequestHandler<GetUserByIdRequest, Result<User>>
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IDesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));

    public async Task<Result<User>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await _desicContext.Users.GetEntityByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogDebug("User with id {UserId} not found", request.UserId);
            return Result.Fail<User>($"User with id {request.UserId} not found");
        }
        return new User
        {
            Id = user.Id,
            Username = user.Username,
        };
    }
}
