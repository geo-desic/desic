using Desic.Application.Common;
using Desic.Application.Common.Extensions;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Desic.Application.Iso3166Countries.List;

public class ListIso3166CountriesRequestHandler(ILogger<ListIso3166CountriesRequestHandler> logger, IApplicationDbContext dbContext) : IRequestHandler<ListIso3166CountriesRequest, Result<ListIso3166CountriesResult>>
{
    private readonly ILogger<ListIso3166CountriesRequestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    internal const bool IncludeTotalCountAllowed = true;
    private const int LogEventId = LogEvents.ListIso3166Countries;
    internal const int MaximumAllowedCount = 300;

    public async Task<Result<ListIso3166CountriesResult>> Handle(ListIso3166CountriesRequest request, CancellationToken cancellationToken)
    {
        request.Pagination.Sanitize(settings: GetRequestSanitizationSettings());

        var query = _dbContext.Iso3166Countries.ApplyFilter(filter: request.Filter).OrderBy(orderingMethod: request.OrderingMethod).SelectToView();
        return await query.ToListResultAsync<Iso3166CountryView, ListIso3166CountriesResult>(pagination: request.Pagination, cancellationToken: cancellationToken);
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
