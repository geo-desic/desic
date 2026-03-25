using Desic.Application.Common;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.EntityTypes.List;

public class ListEntityTypesRequestHandler(ILogger<ListEntityTypesRequestHandler> logger, IApplicationDbContext dbContext) : IRequestHandler<ListEntityTypesRequest, Result<ListEntityTypesResult>>
{
    private readonly ILogger<ListEntityTypesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    internal const bool IncludeTotalCountAllowed = true;
    private const int LogEventId = LogEvents.ListEntityTypes;
    internal const int MaximumAllowedCount = 200;

    public async Task<Result<ListEntityTypesResult>> Handle(ListEntityTypesRequest request, CancellationToken cancellationToken)
    {
        request.Pagination.Sanitize(settings: GetRequestSanitizationSettings());

        var query = _dbContext.EntityTypes.ApplyFilter(filter: request.Filter).OrderBy(orderingMethod: request.OrderingMethod).SelectToModel();
        return await query.ToListResultAsync<EntityType, ListEntityTypesResult>(pagination: request.Pagination, cancellationToken: cancellationToken);
    }

    private PaginationSanitizationSettings GetRequestSanitizationSettings() =>
        new()
        {
            IncludeTotalCountAllowed = IncludeTotalCountAllowed,
            MaximumAllowedCount = MaximumAllowedCount,
            Logger = _logger,
            LogEventId = LogEventId
        };
}
