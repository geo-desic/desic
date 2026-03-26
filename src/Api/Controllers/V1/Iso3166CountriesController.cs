using Desic.Application.Common;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class Iso3166CountriesController(ILogger<Iso3166CountriesController> logger, IMediator mediator) : ApiControllerBase
{
    private readonly ILogger<Iso3166CountriesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [ProducesResponseType(typeof(ListResult<Iso3166CountryView>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ListIso3166CountriesResult>> List([FromQuery] Pagination pagination, [FromQuery] Iso3166CountriesFilter filter, [FromQuery] Iso3166CountriesOrderingMethod orderingMethod = Iso3166CountriesOrderingMethod.NameAsc)
    {
        _logger.LogInformation(LogEvents.ListEntityTypes, $"{nameof(Iso3166CountriesController)}.{nameof(List)}({nameof(Pagination)}, {{{nameof(Iso3166CountriesOrderingMethod)}}}, {nameof(Iso3166CountriesFilter)})", orderingMethod);
        _logger.LogDebug(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {nameof(Pagination.Count)} = {{{nameof(Pagination.Count)}}}; {nameof(Pagination.IncludeTotalCount)} = {{{nameof(Pagination.IncludeTotalCount)}}}; {nameof(Pagination.StartIndex)} = {{{nameof(Pagination.StartIndex)}}}", pagination.Count, pagination.IncludeTotalCount, pagination.StartIndex);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {{@{nameof(Pagination)}}}", pagination);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(Iso3166CountriesFilter)}: {{@{nameof(Iso3166CountriesFilter)}}}", filter);

        var request = new ListIso3166CountriesRequest
        {
            Pagination = pagination,
            OrderingMethod = orderingMethod,
            Filter = filter,
        };
        var result = await _mediator.Send(request);

        return result.Match(onSuccess: u => Ok(u), onFailure: e => Problem(e));
    }
}
