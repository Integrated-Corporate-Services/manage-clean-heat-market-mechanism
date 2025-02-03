using Desnz.Chmm.ApiClients.Http;

namespace Desnz.Chmm.Web.Proxy;

public interface IProxyHttpServiceClient
{
    Task<HttpObjectResponse<dynamic>> HttpSendAsync(HttpRequestMessage httpRequestMessage);
}

public class ProxyHttpServiceClient : HttpServiceClient, IProxyHttpServiceClient
{
    public ProxyHttpServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
    {
    }

    public async Task<HttpObjectResponse<dynamic>> HttpSendAsync(HttpRequestMessage httpRequestMessage)
    {
        return await base.HttpSendAsync<dynamic>(httpRequestMessage);
    }
}
