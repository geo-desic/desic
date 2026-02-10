using Desic.Business.Results;
using Desic.Business.Users.Models;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Business.Users.Commands;

public class CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IMediator mediator, IValidator<UserCreate> validator) : IRequestHandler<CreateUserRequest, Result<User>>
{
    private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException();
    private readonly IValidator<UserCreate> _validator = validator;

    public async Task<Result<User>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (request.User == null)
        {
            _logger.LogDebug("User was not supplied");
            return Result.Fail<User>("User was not supplied");
        }
        var validationResult = _validator.Validate(request.User);
        if (!validationResult.IsValid)
        {
            _logger.LogDebug("User failed validation");
            return validationResult.ToFailResult<User>();
        }

        var query = new EntityFrameworkCore.Users.Queries.GetUserByUsernameRequest { Username = request.User.Username };
        var user = await _mediator.Send(query, cancellationToken);
        if (user != null)
        {
            _logger.LogDebug("A user with username {username} already exists: id = {userId}", user.Username, user.Id);
            return Result.Fail<User>($"A user with username '{user.Username}' already exists");
        }

        user = new EntityFrameworkCore.Entities.User { Username = request.User.Username! };
        var command = new EntityFrameworkCore.Users.Commands.CreateUserRequest { User = user };
        var resultCreate = await _mediator.Send(command, cancellationToken);

        _logger.LogDebug("User was successfully persisted with id = {userId}", resultCreate);

        var result = new User
        {
            Id = user.Id,
        };

        if (request.ReturnResult)
        {
            result.Username = user.Username;
        }
        return Result.Ok(result); // always return a result on success, but if if !ReturnResult only Id will be properly set
    }
}
