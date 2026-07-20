using System.Net.Http.Headers;
using System.Net.Http.Json;
// ReSharper disable ConvertToExtensionBlock

namespace Tooyioo.Tests.Support.Http;

internal static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostJson(
        this HttpClient client,
        string uri,
        object request,
        string? bearerToken)
        => await client.SendJson(uri, request, bearerToken);

    private static async Task<HttpResponseMessage> SendJson(
        this HttpClient client,
        string uri,
        object request,
        string? bearerToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);
        message.Content = JsonContent.Create(request);

        if (bearerToken is not null)
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        
        return await client.SendAsync(message);
    }
}
