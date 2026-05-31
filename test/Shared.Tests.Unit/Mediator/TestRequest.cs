using DispatchR.Abstractions.Send;

namespace Desic.Shared.Tests.Unit.Mediator;

public sealed class TestRequest : IRequest<TestRequest, Task<TestResponse>>
{
    public string? Message { get; set; }
}
