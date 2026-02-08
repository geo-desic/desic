using System.Net;
using Xunit;

namespace Desic.Testing.Integration.Core.Http;

public class StringHttpResponse(HttpResponseMessage response)
{
    private bool _initialized = false;

    public HttpResponseMessage Message { get; } = response;

    public string? Content { get; private set; }

    public HttpStatusCode StatusCode => Message.StatusCode;

    public virtual async Task<StringHttpResponse> InitializeContent()
    {
        if (_initialized) return this;
        Content = await Message.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        _initialized = true;
        return this;
    }
}