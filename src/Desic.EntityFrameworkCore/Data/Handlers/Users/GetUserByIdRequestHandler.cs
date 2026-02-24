using Desic.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Data.Handlers.Users;

public class GetUserByIdRequestHandler(DesicContext desicContext, ILogger<GetUserByIdRequestHandler> logger) : IRequestHandler<GetUserByIdRequest, User?>
{
    private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
    private readonly ILogger<GetUserByIdRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<User?> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        return await _desicContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
    }
}
