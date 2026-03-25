using Desic.Application.Common;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class EntityTypesController(ILogger<EntityTypesController> logger, IMediator mediator) : ApiControllerBase
{
    private readonly ILogger<EntityTypesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet]
    [ProducesResponseType(typeof(ListResult<EntityType>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ListResult<EntityType>>> List([FromQuery] Pagination pagination, [FromQuery] EntityTypesOrderingMethod orderingMethod, [FromQuery] EntityTypesFilter filter)
    {
        _logger.LogInformation(LogEvents.ListEntityTypes, $"{nameof(EntityTypesController)}.{nameof(List)}({nameof(Pagination)}, {{{nameof(EntityTypesOrderingMethod)}}}, {nameof(EntityTypesFilter)})", orderingMethod);
        _logger.LogDebug(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {nameof(Pagination.Count)} = {{{nameof(Pagination.Count)}}}; {nameof(Pagination.IncludeTotalCount)} = {{{nameof(Pagination.IncludeTotalCount)}}}; {nameof(Pagination.StartIndex)} = {{{nameof(Pagination.StartIndex)}}}", pagination.Count, pagination.IncludeTotalCount, pagination.StartIndex);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(Pagination)}: {{@{nameof(Pagination)}}}", pagination);
        _logger.LogTrace(LogEvents.ListEntityTypes, $"{nameof(EntityTypesFilter)}: {{@{nameof(EntityTypesFilter)}}}", filter);

        var request = new ListEntityTypesRequest
        {
            Pagination = pagination,
            OrderingMethod = orderingMethod,
            Filter = filter,
        };
        var result = await _mediator.Send(request);

        return result.Match(onSuccess: u => Ok(u), onFailure: e => Problem(e));
    }
}
