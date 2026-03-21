using Desic.Application.Common;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;
using Desic.Domain.Labels;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Users.Create;

public class CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IApplicationDbContext dbContext, IValidator<CreateUser> validator) : IRequestHandler<CreateUserRequest, Result<CreateUserResult>>
{
    private readonly ILogger<CreateUserRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IValidator<CreateUser> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    private const int LogEventId = LogEvents.CreateUser;

    public async Task<Result<CreateUserResult>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (!_validator.InstanceIsValid(request.Model, out var error)) return error!;

        if (await _dbContext.Users.AnyAsync(x => x.Username == request.Model.Username, cancellationToken))
        {
            _logger.LogDebug(LogEventId, "A user with username '{Username}' already exists", request.Model.Username);
            return new Error($"A user with username '{request.Model.Username}' already exists");
        }

        var user = new Domain.Users.User
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            Username = request.Model.Username!
        };

        user.SetCreatedAndModifiedBy(by: SystemLabels.System, on: DateTime.UtcNow);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(LogEventId, "User was successfully persisted with id = {UserId}", user.Id);

        var result = new CreateUserResult { Id = user.Id };

        if (!request.ReturnRepresentation) return result;

        result.Model = user.ToDto();

        return result;
    }
}
