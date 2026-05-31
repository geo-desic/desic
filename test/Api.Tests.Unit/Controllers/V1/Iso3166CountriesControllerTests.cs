using AwesomeAssertions;
using Desic.Api.Controllers.V1;
using Desic.Api.Dtos;
using Desic.Application.Common;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Desic.Application.Iso3166Countries;
using Desic.Application.Iso3166Countries.List;
using Desic.Shared.Extensions;
using DispatchR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;

namespace Desic.Api.Tests.Unit.Controllers.V1;

public class Iso3166CountriesControllerTests
{
    private readonly ILogger<Iso3166CountriesController> _logger = NullLogger<Iso3166CountriesController>.Instance;
    private readonly Mock<IMediator> _mediator = new();

    public class Iso3166CountriesControllerTests001 : Iso3166CountriesControllerTests
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
            var orderingMethodFromQuery = new Dtos.OrderingMethodFromQuery<Iso3166CountriesOrderingProperty>
            {
                Asc = [false, true],
                OrderBy = [Iso3166CountriesOrderingProperty.Alpha3, Iso3166CountriesOrderingProperty.IsoId],
            };
            var expectedOrderingMethod = new OrderingMethod<Iso3166CountriesOrderingProperty>
            {
                OrderBy =
                [
                    new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = false, Property = Iso3166CountriesOrderingProperty.Alpha3},
                    new OrderBy<Iso3166CountriesOrderingProperty> { Ascending = true, Property = Iso3166CountriesOrderingProperty.IsoId},
                ],
            };

            var filter = new Iso3166CountriesFilter
            {
                Alpha2 = "aa",
                Alpha3 = "aaa",
                Id = 1.ToGuid(),
                IsoId = 1,
                Name = "Name1",
                NameContains = "Name1",
            };
            var sendResult = GetSendResult(items: GetItems(count: 1), pagination: pagination);
            Setup(result: sendResult);
            var controller = NewController();

            // act
            var result = (await controller.List(pagination: pagination, filter: filter, orderingMethodFromQuery: orderingMethodFromQuery, cancellationToken: TestContext.Current.CancellationToken))?.Result as OkObjectResult;

            // assert
            _mediator.Verify(x => x.Send(It.Is<ListIso3166CountriesRequest>(x =>
                x.Pagination.IsEquivalentTo(pagination)
                && x.OrderingMethod.IsEquivalentTo(expectedOrderingMethod)
                && IsEquivalentTo(source: x.Filter, compare: filter)), It.IsAny<CancellationToken>()), Times.Once());
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var objectResult = result.Value as ListIso3166CountriesResult;
            objectResult.Should().BeEquivalentTo(sendResult.Value);
        }
    }

    public class Iso3166CountriesControllerTests002 : Iso3166CountriesControllerTests
    {
        [Fact]
        public async Task List_InvalidRequestInvalidOrderingMethodFromQuery_400BadRequestWithExpectedProblemDetails()
        {
            // arrange
            var sendResult = GetSendResult(items: GetItems(count: 1), pagination: new Pagination());
            Setup(result: sendResult);
            var controller = NewController();
            var orderingMethodFromQueryInvalid = new OrderingMethodFromQuery<Iso3166CountriesOrderingProperty>
            {
                Asc = [true, true], // has more items than OrderBy which is invalid
                OrderBy = [Iso3166CountriesOrderingProperty.Name],
            };

            // act
            var result = (await controller.List(pagination: new(), filter: new(), orderingMethodFromQuery: orderingMethodFromQueryInvalid, cancellationToken: TestContext.Current.CancellationToken))?.Result as ObjectResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var objectResult = result.Value as ProblemDetails;
            objectResult.Should().NotBeNull();
            objectResult.Status.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }

    public class Iso3166CountriesControllerTests003 : Iso3166CountriesControllerTests
    {
        [Fact]
        public async Task List_InvalidRequestSendReturnsError_400BadRequestWithExpectedProblemDetails()
        {
            // arrange
            Setup(result: new(new Error("Test")));
            var controller = NewController();

            // act
            var result = (await controller.List(pagination: new(), filter: new(), orderingMethodFromQuery: new(), cancellationToken: TestContext.Current.CancellationToken))?.Result as ObjectResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var objectResult = result.Value as ProblemDetails;
            objectResult.Should().NotBeNull();
            objectResult.Status.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }

    private static Result<ListIso3166CountriesResult> GetSendResult(IEnumerable<Iso3166CountryView>? items = null, IPagination? pagination = null)
    {
        return new Result<ListIso3166CountriesResult>(new ListIso3166CountriesResult
        {
            Items = items?.ToList() ?? [],
            StartIndex = pagination?.StartIndex ?? 0,
            TotalCount = (pagination?.IncludeTotalCount ?? false) ? 100 : null,
        });
    }

    private void Setup(Result<ListIso3166CountriesResult>? result = null)
    {
        result ??= new((ListIso3166CountriesResult)null!);
        _mediator.Setup(x => x.Send(It.IsAny<ListIso3166CountriesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result.Value);
    }

    private Iso3166CountriesController NewController()
    {
        return new Iso3166CountriesController(logger: _logger, mediator: _mediator.Object);
    }

    private static IEnumerable<Iso3166CountryView> GetItems(int count)
    {
        for (var i = 0; i < count; ++i) // count must be <= 26 for alpha2 & alpha3 values to be unique using generation method below
        {
            yield return new Iso3166CountryView
            {
                Alpha2 = new string((char)('a' + i), Domain.Iso3166Countries.Iso3166Country.LengthAlpha2),
                Alpha3 = new string((char)('a' + i), Domain.Iso3166Countries.Iso3166Country.LengthAlpha3),
                Id = i.ToGuid(),
                IsoId = i,
                Name = $"Name{i}",
            };
        }
    }

    private static bool IsEquivalentTo(Iso3166CountriesFilter source, Iso3166CountriesFilter compare)
    {
        return source.Alpha2 == compare.Alpha2 && source.Alpha3 == compare.Alpha3 && source.Id == compare.Id && source.IsoId == compare.IsoId && source.Name == compare.Name;
    }
}
