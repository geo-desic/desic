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
    internal const bool IncludeTotalCount = true;
    private const int LogEventId = LogEvents.ListEntityTypes;
    internal const int MaximumAllowedCount = 200;

    public async Task<Result<ListEntityTypesResult>> Handle(ListEntityTypesRequest request, CancellationToken cancellationToken)
    {
        request.Sanitize(settings: GetRequestSanitizationSettings());

        var query = _dbContext.EntityTypes.Select(x => new EntityType { Name = x.Name, Key = x.Key }).OrderBy(orderingMethod: request.OrderingMethod);
        return await query.ToListResultAsync<EntityType, ListEntityTypesResult>(startIndex: request.StartIndex, takeCount: request.Count, includeTotalCount: IncludeTotalCount, cancellationToken: cancellationToken);
    }

    private ListRequestSanitizationSettings GetRequestSanitizationSettings() =>
        new()
        {
            MaximumAllowedCount = MaximumAllowedCount,
            Logger = _logger,
            LogEventId = LogEventId
        };
}
