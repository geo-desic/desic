using Desic.Application.Common;
using Desic.Application.Common.Helpers;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequestHandler(ILogger<ListEntityTypesRequestHandler> logger, IApplicationDbContext dbContext) : IRequestHandler<ListEntityTypesRequest, Result<PaginatedList<EntityType>>>
{
    private readonly ILogger<ListEntityTypesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<Result<PaginatedList<EntityType>>> Handle(ListEntityTypesRequest request, CancellationToken cancellationToken)
    {
        if (request.Offset < 0)
        {
            _logger.LogWarning(LogEvents.EntityTypeList, "Negative offset is not supported, defaulting offset to 0");
            request.Offset = 0;
        }

        var query = _dbContext.EntityTypes.Select(x => new EntityType { Name = x.Name, Key = x.Key }).OrderBy(x => x.Name);
        return await query.ToPaginatedList(offset: request.Offset, cancellationToken: cancellationToken);
    }
}
