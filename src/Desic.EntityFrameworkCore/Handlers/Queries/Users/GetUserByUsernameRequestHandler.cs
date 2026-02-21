using Desic.Data.Entities;
using Desic.Data.Requests.Queries.Users;
using Desic.EntityFrameworkCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Desic.EntityFrameworkCore.Handlers.Queries.Users;

public class GetUserByUsernameRequestHandler(DesicContext desicContext, ILogger<GetUserByUsernameRequestHandler> logger) : IRequestHandler<GetUserByUsernameRequest, User?>
{
    private readonly DesicContext _desicContext = desicContext ?? throw new ArgumentNullException(nameof(desicContext));
    private readonly ILogger<GetUserByUsernameRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<User?> Handle(GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        return await _desicContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
    }

}
