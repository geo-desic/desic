using DispatchR.Abstractions.Send;

namespace Desic.Shared.Tests.Unit.Mediator;

public sealed class TestRequestHandler : IRequestHandler<TestRequest, Task<TestResponse>>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse { Message = request.Message + " Response" });
    }
}
