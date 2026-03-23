using Xunit;

namespace Desic.Testing.Integration.Http;

public static class HttpClientHelpers
{
    public static async Task<StringHttpResponse> SendAsyncAndReadResponseAsString(this HttpClient httpClient, FluentHttpRequest request)
    {
        Console.WriteLine($"Attempting http request: {GetFullUrl(baseAddress: httpClient.BaseAddress, requestUri: request.HttpRequestMessage.RequestUri)}");
        var response = await httpClient.SendAsync(request.HttpRequestMessage, TestContext.Current.CancellationToken);
        var result = new StringHttpResponse(response);
        await result.InitializeContent();
        return result;
    }

    public static async Task<ObjectHttpResponse<T>> SendAsyncAndReadResponseAsJson<T>(this HttpClient httpClient, FluentHttpRequest request)
    {
        Console.WriteLine($"Attempting http request: {GetFullUrl(baseAddress: httpClient.BaseAddress, requestUri: request.HttpRequestMessage.RequestUri)}");
        var response = await httpClient.SendAsync(request.HttpRequestMessage, TestContext.Current.CancellationToken);
        var result = new ObjectHttpResponse<T>(response);
        await result.InitializeContentAsJson();
        return result;
    }

    private static string? GetFullUrl(Uri? baseAddress, Uri? requestUri)
    {
        Uri? uri = null;
        if (baseAddress != null && requestUri != null) uri = new Uri(baseAddress, requestUri);
        else if (baseAddress != null) uri = baseAddress;
        else if (requestUri != null) uri = requestUri;
        return uri?.ToString();
    }
}
