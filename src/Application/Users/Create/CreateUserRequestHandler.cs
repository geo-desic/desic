using Desic.Application.Common;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;
using Desic.Domain.Results;
using Desic.Domain.Tags;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Create;

public class CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IDesicContext desicContext, IValidator<UserCreate> validator) : IRequestHandler<CreateUserRequest, Result<CreateResult<User>>>
{
    private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IDesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
    private readonly IValidator<UserCreate> _validator = validator;

    public async Task<Result<CreateResult<User>>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (_validator.ValidationException(request.User) is Common.Exceptions.ValidationException e) return e;

        if (await _desicContext.Users.AnyAsync(x => x.Username == request.User.Username, cancellationToken))
        {
            _logger.LogDebug("A user with username '{Username}' already exists", request.User.Username);
            return new ValidationException($"A user with username '{request.User.Username}' already exists");
        }

        var user = new Domain.Users.User
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            Username = request.User.Username!
        };

        user.SetCreatedAndModifiedBy(by: SystemTags.Get(SystemTag.System), on: DateTime.UtcNow);

        _desicContext.Users.Add(user);

        await _desicContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("User was successfully persisted with id = {UserId}", user.Id);

        var result = new CreateResult<User> { Id = user.Id };

        if (!request.ReturnRepresentation) return result;

        result.Entity = new User
        {
            Id = user.Id,
            Username = user.Username,
        };

        return result;
    }
}
