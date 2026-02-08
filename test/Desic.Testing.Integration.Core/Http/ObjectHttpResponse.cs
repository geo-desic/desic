using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Desic.Testing.Integration.Core.Http;

public class ObjectHttpResponse<T>(HttpResponseMessage response)
{
    private bool _initialized = false;

    public HttpResponseMessage Message { get; } = response;

    public static JsonSerializerOptions DefaultJsonSerializerOptions { get; }

    static ObjectHttpResponse()
    {
        DefaultJsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public T? Content { get; private set; }

    public HttpStatusCode StatusCode => Message.StatusCode;

    public async Task<ObjectHttpResponse<T>> InitializeContentAsJson()
    {
        return await InitializeContentAsJson(DefaultJsonSerializerOptions);
    }

    public async Task<ObjectHttpResponse<T>> InitializeContentAsJson(JsonSerializerOptions jsonSerializerOptions)
    {
        if (_initialized) return this;
        Content = await Message.Content.ReadFromJsonAsync<T>(jsonSerializerOptions, TestContext.Current.CancellationToken);
        _initialized = true;
        return this;
    }
}
