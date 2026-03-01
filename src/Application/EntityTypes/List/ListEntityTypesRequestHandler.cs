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
    internal const bool IncludeTotalCount = true;
    internal const int MaximumAllowedCount = 100;

    public async Task<Result<PaginatedList<EntityType>>> Handle(ListEntityTypesRequest request, CancellationToken cancellationToken)
    {
        if (request.StartIndex < 0)
        {
            _logger.LogWarning(LogEvents.EntityTypeList, "Negative offset is not supported, defaulting offset to 0");
            request.StartIndex = 0;
        }
        if (request.Count > MaximumAllowedCount)
        {
            _logger.LogInformation(LogEvents.EntityTypeList, "Requested count is greater than maximum allowed, capping at the maximum: {RequestCount} > {MaximumAllowedCount}", request.Count, MaximumAllowedCount);
            request.Count = MaximumAllowedCount;
        }

        var query = _dbContext.EntityTypes.Select(x => new EntityType { Name = x.Name, Key = x.Key }).OrderBy(x => x.Name);
        return await query.ToPaginatedList(startIndex: request.StartIndex, takeCount: request.Count, includeTotalCount: IncludeTotalCount, cancellationToken: cancellationToken);
    }
}
