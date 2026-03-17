using MediatR;

namespace Desic.Shared.Tests.Unit.Mediator;

public class TestRequest : IRequest<TestResponse>
{
    public string? Message { get; set; }
}
