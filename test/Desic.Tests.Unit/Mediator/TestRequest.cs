using MediatR;

namespace Desic.Tests.Unit.Mediator;

public class TestRequest : IRequest<TestResponse>
{
    public string? Message { get; set; }
}
