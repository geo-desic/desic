using MediatR;

namespace Desic.Shared.Tests.Unit.Mediator;

public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse { Message = request.Message + " Response" });
    }
}
