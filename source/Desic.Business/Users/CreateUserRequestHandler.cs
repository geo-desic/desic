using Desic.Business.Results;
using Desic.Business.Users.Models;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Business.Users
{
    public class CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IMediator mediator, IValidator<UserCreate> validator) : IRequestHandler<CreateUserRequest, Result<User>>
    {
        private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException();
        private readonly IValidator<UserCreate> _validator = validator;

        public async Task<Result<User>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { { new("handlerType", nameof(CreateUserRequestHandler)) } });
            _logger.LogTrace("Handling {requestType}: {@request}", nameof(CreateUserRequest), request);
            _logger.LogDebug("Handling {requestType}", nameof(CreateUserRequest));

            if (request.User == null)
            {
                return Result.Fail("User to create was not supplied");
            }
            var validationResult = _validator.Validate(request.User);
            if (!validationResult.IsValid)
            {
                return validationResult.ToFailResult<User>();
            }

            var query = new EntityFrameworkCore.Users.Queries.GetUserByUsernameRequest { Username = request.User.Username };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(EntityFrameworkCore.Users.Queries.GetUserByUsernameRequest), query);
            _logger.LogDebug("Invoking {requestType}", nameof(EntityFrameworkCore.Users.Queries.GetUserByUsernameRequest));
            var user = await _mediator.Send(query, cancellationToken);
            if (user != null)
            {
                _logger.LogInformation("A user with username {username} already exists: id = {id}", user.Username, user.Id);
                return Result.Fail($"A user with username '{user.Username}' already exists");
            }

            user = new EntityFrameworkCore.Entities.User { Username = request.User.Username! };
            var command = new EntityFrameworkCore.Users.Commands.CreateUserRequest { User = user };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(EntityFrameworkCore.Users.Commands.CreateUserRequest), command);
            _logger.LogDebug("Invoking {requestType}", nameof(EntityFrameworkCore.Users.Commands.CreateUserRequest));
            var commandResult = await _mediator.Send(command, cancellationToken);

            if (request.ReturnResult)
            {
                var result = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                };
                return Result.Ok(result);
            }
            return Result.Ok();
        }
    }
}
