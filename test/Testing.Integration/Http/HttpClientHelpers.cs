using Xunit;

namespace Desic.Testing.Integration.Http;

public static class HttpClientHelpers
{
    public static async Task<StringHttpResponse> SendAsyncAndReadResponseAsString(this HttpClient httpClient, FluentHttpRequest request)
    {
        var response = await httpClient.SendAsync(request.HttpRequestMessage, TestContext.Current.CancellationToken);
        var result = new StringHttpResponse(response);
        await result.InitializeContent();
        return result;
    }

    public static async Task<ObjectHttpResponse<T>> SendAsyncAndReadResponseAsJson<T>(this HttpClient httpClient, FluentHttpRequest request)
    {
        var response = await httpClient.SendAsync(request.HttpRequestMessage, TestContext.Current.CancellationToken);
        var result = new ObjectHttpResponse<T>(response);
        await result.InitializeContentAsJson();
        return result;
    }
}
