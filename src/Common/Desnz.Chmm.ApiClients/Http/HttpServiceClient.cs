using Desnz.Chmm.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Desnz.Chmm.ApiClients.Http;

public class HttpServiceClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public HttpServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    protected async Task<HttpObjectResponse<T>> HttpPostAsync<T>(string requestUri, object content, bool authenticate = true, string? token = null)
        where T : class
    {
        return await HttpSendAsync<T>(requestUri, HttpMethod.Post, authenticate, content, token);
    }

    protected async Task<HttpObjectResponse<T>> HttpPutAsync<T>(string requestUri, object content, bool authenticate = true)
        where T : class
    {
        return await HttpSendAsync<T>(requestUri, HttpMethod.Put, authenticate, content);
    }

    protected async Task<HttpObjectResponse<T>> HttpGetAsync<T>(string requestUri, bool authenticate = true, string? token = null)
        where T : class
    {
        return await HttpSendAsync<T>(requestUri, HttpMethod.Get, authenticate, token: token);
    }

    protected async Task<HttpObjectResponse<T>> HttpGetAsync<T>(string requestUri, object query, bool authenticate = true, string? token = null)
        where T : class
    {
        return await HttpSendAsync<T>(requestUri, HttpMethod.Get, authenticate, content: query, token: token);
    }

    protected async Task<HttpObjectResponse<T>> HttpSendAsync<T>(string requestUri, HttpMethod httpMethod, bool authenticate = true, object? content = null, string? token = null)
        where T : class
    {
        var request = new HttpRequestMessage(httpMethod, requestUri);
        return await HttpSendAsync<T>(request, authenticate, content, token);
    }

    protected async Task<HttpObjectResponse<T>> HttpSendAsync<T>(HttpRequestMessage request, bool authenticate = true, object? content = null, string? token = null)
       where T : class
    {
        if (authenticate)
        {
            if (_httpContextAccessor.HttpContext is HttpContext context)
            {
                token ??= await context.GetTokenAsync(IdentityConstants.Authentication.TokenName);
                // TODO: Temporary solution to pass the CHMM token in calls between microservices
                token ??= await context.GetTokenAsync("access_token");

                if (token is not null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // If we've got a date-time-override header, we need to persist that further down the stack.
                if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("date-time-override"))
                {
                    request.Headers.Add("date-time-override", _httpContextAccessor.HttpContext.Request.Headers["date-time-override"].ToString());
                }
            }
        }

        var methodsWithContent = new List<HttpMethod>() { HttpMethod.Post, HttpMethod.Put, HttpMethod.Put, HttpMethod.Get };
        if (content != null && methodsWithContent.Contains(request.Method))
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.SendAsync(request);

        var result = await HttpObjectResponseFactory.DetermineSuccess<T>(response);
        return result;
    }
}
