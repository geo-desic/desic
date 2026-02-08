using System.Net.Http.Headers;

namespace Desic.Testing.Integration.Core.Http;

public class FluentHttpRequest(HttpMethod httpMethod, string? requestUri)
{
    public HttpRequestMessage HttpRequestMessage { get; } = new(httpMethod, requestUri);

    public FluentHttpRequest AddAcceptHeader(MediaTypeWithQualityHeaderValue value)
    {
        HttpRequestMessage.Headers.Accept.Add(value);
        return this;
    }

    public FluentHttpRequest AddAcceptHeader(string value, double? quality)
    {
        var acceptValue = new MediaTypeWithQualityHeaderValue(value);
        if (quality != null)
        {
            acceptValue.Quality = quality;
        }
        return AddAcceptHeader(acceptValue);
    }

    public FluentHttpRequest AddHeader(string name, string? value)
    {
        HttpRequestMessage.Headers.Add(name, value);
        return this;
    }

    public FluentHttpRequest AddHeader(string name, IEnumerable<string?> values)
    {
        HttpRequestMessage.Headers.Add(name, values);
        return this;
    }

    public FluentHttpRequest RemoveHeader(string name)
    {
        HttpRequestMessage.Headers.Remove(name);
        return this;
    }

    public FluentHttpRequest SetAuthorizationHeader(AuthenticationHeaderValue value)
    {
        HttpRequestMessage.Headers.Authorization = value;
        return this;
    }

    public FluentHttpRequest SetAuthorizationHeader(string scheme, string? parameter)
    {
        var value = new AuthenticationHeaderValue(scheme, parameter);
        return SetAuthorizationHeader(value);
    }
}
