using Desic.Application.Results;
using Desic.Domain.Shared;
using Desic.Domain.Shared.Entities;
using Desic.Domain.Tags;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Create;

public class CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IRepository<Domain.Users.User> repository, IValidator<UserCreate> validator) : IRequestHandler<CreateUserRequest, Result<User>>
{
    private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IRepository<Domain.Users.User> _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

        if (await _repository.AnyAsync(x => x.Username == request.User.Username))
        {
            _logger.LogDebug("A user with username '{Username}' already exists", request.User.Username);
            return Result.Fail<User>($"A user with username '{request.User.Username}' already exists");
        }

        var user = new Domain.Users.User
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            Username = request.User.Username!
        };

        user.SetCreatedAndModifiedBy(by: SystemTags.Get(SystemTag.System), on: DateTime.UtcNow);

        await _repository.AddAsync(user, cancellationToken);

        _logger.LogDebug("User was successfully persisted with id = {UserId}", user.Id);

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
