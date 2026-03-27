using AwesomeAssertions;
using Desic.Api.Controllers.V1;
using Desic.Application.Common;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Application.EntityTypes.List;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;

namespace Desic.Api.Tests.Unit.Controllers.V1;

public class EntityTypesControllerTests
{
    private readonly ILogger<EntityTypesController> _logger = NullLogger<EntityTypesController>.Instance;
    private readonly Mock<IMediator> _mediator = new();

    public class EntityTypesControllerTests001 : EntityTypesControllerTests
    {
        [Fact]
        public async Task List_ValidRequestWithNonDefaultInputs_200OkWithExpectedResultAndSendHasExpectedValues()
        {
            // arrange
            var pagination = new Pagination
            {
                StartIndex = 1,
                Count = 10,
                IncludeTotalCount = true,
            };
            var orderingMethod = EntityTypesOrderingMethod.KeyDesc;
            var filter = new EntityTypesFilter
            {
                Key = "aaaa",
                Name = "Name1",
            };
            var sendResult = GetSendResult(items: GetItems(count: 1), pagination: pagination);
            Setup(result: sendResult);
            var controller = NewController();

            // act
            var result = (await controller.List(pagination: pagination, filter: filter, orderingMethod: orderingMethod))?.Result as OkObjectResult;

            // assert
            _mediator.Verify(x => x.Send(It.Is<ListEntityTypesRequest>(x => IsEquivalentTo(source: x.Pagination, compare: pagination) && x.OrderingMethod == orderingMethod && IsEquivalentTo(source: x.Filter, compare: filter))), Times.Once());
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var objectResult = result.Value as ListEntityTypesResult;
            objectResult.Should().BeEquivalentTo(sendResult.Value);
        }
    }

    public class EntityTypesControllerTests002 : EntityTypesControllerTests
    {
        [Fact]
        public async Task List_InvalidRequestSendReturnsError_400BadRequestWithExpectedProblemDetails()
        {
            // arrange
            Setup(result: new(new Error("Test")));
            var controller = NewController();

            // act
            var result = (await controller.List(pagination: new(), filter: new(), orderingMethod: default))?.Result as ObjectResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var objectResult = result.Value as ProblemDetails;
            objectResult.Should().NotBeNull();
            objectResult.Status.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }

    private void Setup(Result<ListEntityTypesResult>? result = null)
    {
        result ??= new((ListEntityTypesResult)null!);
        _mediator.Setup(x => x.Send(It.IsAny<ListEntityTypesRequest>())).ReturnsAsync(result.Value);
    }

    private static Result<ListEntityTypesResult> GetSendResult(IEnumerable<EntityType>? items = null, IPagination? pagination = null)
    {
        return new Result<ListEntityTypesResult>(new ListEntityTypesResult
        {
            Items = items?.ToList() ?? [],
            StartIndex = pagination?.StartIndex ?? 0,
            TotalCount = (pagination?.IncludeTotalCount ?? false) ? 100 : null,
        });
    }

    private EntityTypesController NewController()
    {
        return new EntityTypesController(logger: _logger, mediator: _mediator.Object);
    }

    private static IEnumerable<EntityType> GetItems(int count)
    {
        for (var i = 0; i < count; ++i) // count must be <= 26 for key values to be unique using generation method below
        {
            yield return new EntityType
            {
                Key = new string((char)('a' + i),
                Domain.EntityTypes.EntityType.LengthKey),
                Name = $"Name{i}",
            };
        }
    }

    private static bool IsEquivalentTo(IPagination source, IPagination compare)
    {
        return source.StartIndex == compare.StartIndex && source.Count == compare.Count && source.IncludeTotalCount == compare.IncludeTotalCount;
    }

    private static bool IsEquivalentTo(EntityTypesFilter source, EntityTypesFilter compare)
    {
        return source.Key == compare.Key && source.Name == compare.Name;
    }
}
