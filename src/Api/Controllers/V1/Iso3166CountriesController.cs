using Desic.Api.Dtos;
using Desic.Application.Common;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using DispatchR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class Iso3166CountriesController(ILogger<Iso3166CountriesController> logger, IMediator mediator) : ApiControllerBase
{
    private readonly ILogger<Iso3166CountriesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [ProducesResponseType(typeof(ListIso3166CountriesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ListIso3166CountriesResult>> List([FromQuery] Pagination pagination, [FromQuery] Iso3166CountriesFilter filter, [FromQuery] OrderingMethodFromQuery<Iso3166CountriesOrderingProperty> orderingMethodFromQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LogEvents.ListIso3166Countries, $"{nameof(Iso3166CountriesController)}.{nameof(List)}({nameof(Pagination)}, {nameof(Iso3166CountriesFilter)}, {nameof(OrderingMethodFromQuery<>)})");
        _logger.LogDebug(LogEvents.ListIso3166Countries, $"{nameof(Pagination)}: {nameof(Pagination.Count)} = {{{nameof(Pagination.Count)}}}; {nameof(Pagination.IncludeTotalCount)} = {{{nameof(Pagination.IncludeTotalCount)}}}; {nameof(Pagination.StartIndex)} = {{{nameof(Pagination.StartIndex)}}}", pagination.Count, pagination.IncludeTotalCount, pagination.StartIndex);
        _logger.LogTrace(LogEvents.ListIso3166Countries, $"{nameof(Pagination)}: {{@{nameof(Pagination)}}}", pagination);
        _logger.LogTrace(LogEvents.ListIso3166Countries, $"{nameof(Iso3166CountriesFilter)}: {{@{nameof(Iso3166CountriesFilter)}}}", filter);
        _logger.LogTrace(LogEvents.ListIso3166Countries, $"{nameof(OrderingMethodFromQuery<>)}: {{@{nameof(OrderingMethodFromQuery<>)}}}", orderingMethodFromQuery);

        var orderingMethod = ConvertOrderingMethod(orderingMethodFromQuery, out var problemResult, _logger, LogEvents.ListIso3166Countries);
        if (orderingMethod == null) return problemResult!;

        var request = new ListIso3166CountriesRequest
        {
            Pagination = pagination,
            OrderingMethod = orderingMethod,
            Filter = filter,
        };
        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(onSuccess: r => Ok(r), onFailure: e => Problem(e));
    }
}
