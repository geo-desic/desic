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
    public async Task<ActionResult<ListResult<EntityType>>> List([FromQuery] ListEntityTypesRequest request)
    {
        _logger.LogInformation(LogEvents.ListEntityTypes, $"{nameof(EntityTypesController)}.{nameof(List)}({nameof(request)}) with {nameof(request.StartIndex)} = {{{nameof(request.StartIndex)}}}, {nameof(request.Count)} = {{{nameof(request.Count)}}}, {nameof(request.OrderingMethod)} = {{{nameof(request.OrderingMethod)}}})", request.StartIndex, request.Count, request.OrderingMethod);

        var result = await _mediator.Send(request);

        return result.Match(onSuccess: u => Ok(u), onFailure: e => Problem(e));
    }
}
