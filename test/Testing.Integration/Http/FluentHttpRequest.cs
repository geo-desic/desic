using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace Desic.Testing.Integration.Http;

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

    public FluentHttpRequest AddQueryStringParameter(string name, string? value)
    {
        if (HttpRequestMessage.RequestUri == null) throw new InvalidOperationException($"Cannot add query string parameters when {nameof(HttpRequestMessage.RequestUri)} is null");
        if (HttpRequestMessage.RequestUri.IsAbsoluteUri)
        {
            var builder = new UriBuilder(HttpRequestMessage.RequestUri);
            var queryParameters = HttpUtility.ParseQueryString(builder.Query);
            queryParameters[name] = value;
            builder.Query = queryParameters.ToString();
            HttpRequestMessage.RequestUri = builder.Uri;
        }
        else
        {
            const string dummyBaseUriString = "http://dummyhost:80/";
            var currentUriString = HttpRequestMessage.RequestUri.ToString();
            var uri = new Uri(dummyBaseUriString + currentUriString.TrimStart('/'));
            var builder = new UriBuilder(uri);
            var queryParameters = HttpUtility.ParseQueryString(builder.Query);
            builder.Query = string.Empty;
            var currentUriStringWithoutQueryParameters = string.Concat("/", builder.ToString().AsSpan(start: dummyBaseUriString.Length));
            queryParameters[name] = value;
            HttpRequestMessage.RequestUri = new Uri($"{currentUriStringWithoutQueryParameters}?{queryParameters}", UriKind.Relative);
        }
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

    public FluentHttpRequest SetJsonContent<T>(T content)
    {
        HttpRequestMessage.Content = JsonContent.Create(content, options: Constants.DefaultJsonSerializerOptions);
        return this;
    }
}
