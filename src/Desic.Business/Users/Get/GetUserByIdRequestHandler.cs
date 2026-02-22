using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Business.Users.Get;

public class GetUserByIdRequestHandler(ILogger<GetUserByIdRequestHandler> logger, IMediator mediator) : IRequestHandler<GetUserByIdRequest, Result<User>>
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<User>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var query = new Data.Requests.Queries.Users.GetUserByIdRequest { UserId = request.UserId };
        var user = await _mediator.Send(query, cancellationToken);

        if (user == null)
        {
            _logger.LogDebug("User with id {UserId} not found", request.UserId);
            return Result.Fail($"User with id {request.UserId} not found");
        }
        return new User
        {
            Id = user.Id,
            Username = user.Username,
        };
    }
}
