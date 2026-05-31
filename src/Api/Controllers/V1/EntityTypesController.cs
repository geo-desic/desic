using Desic.Api.Dtos;
using Desic.Application.Common;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using DispatchR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class EntityTypesController(ILogger<EntityTypesController> logger, IMediator mediator) : ApiControllerBase
{
    private readonly ILogger<EntityTypesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [ProducesResponseType(typeof(ListEntityTypesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ListEntityTypesResult>> List([FromQuery] Pagination pagination, [FromQuery] EntityTypesFilter filter, [FromQuery] OrderingMethodFromQuery<EntityTypesOrderingProperty> orderingMethodFromQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LogEvents.ListEntityTypes, $"{nameof(EntityTypesController)}.{nameof(List)}({nameof(Pagination)}, {nameof(EntityTypesFilter)}, {nameof(OrderingMethodFromQuery<>)})");
        _logger.LogDebug(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {nameof(Pagination.Count)} = {{{nameof(Pagination.Count)}}}; {nameof(Pagination.IncludeTotalCount)} = {{{nameof(Pagination.IncludeTotalCount)}}}; {nameof(Pagination.StartIndex)} = {{{nameof(Pagination.StartIndex)}}}", pagination.Count, pagination.IncludeTotalCount, pagination.StartIndex);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {{@{nameof(Pagination)}}}", pagination);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(EntityTypesFilter)}: {{@{nameof(EntityTypesFilter)}}}", filter);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(OrderingMethodFromQuery<>)}: {{@{nameof(OrderingMethodFromQuery<>)}}}", orderingMethodFromQuery);

        var orderingMethod = ConvertOrderingMethod(orderingMethodFromQuery, out var problemResult, _logger, LogEvents.ListEntityTypes);
        if (orderingMethod == null) return problemResult!;

        var request = new ListEntityTypesRequest
        {
            Pagination = pagination,
            OrderingMethod = orderingMethod,
            Filter = filter,
        };
        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(onSuccess: r => Ok(r), onFailure: e => Problem(e));
    }
}
