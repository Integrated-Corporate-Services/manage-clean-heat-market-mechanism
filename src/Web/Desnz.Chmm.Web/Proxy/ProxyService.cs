using Amazon.Util;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;

namespace Desnz.Chmm.Web.Proxy;

public interface IProxyService
{
    Task<HttpRequestMessage> PrepareProxyRequest();
}

public class ProxyService : IProxyService
{
    private readonly ILogger<ProxyService> _logger;
    private readonly ProxyConfig _proxyConfig;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProxyService(
        ILogger<ProxyService> logger,
        IOptions<ProxyConfig> proxyConfig,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _proxyConfig = proxyConfig.Value
            ?? throw new ArgumentNullException(nameof(proxyConfig));
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _configuration = configuration
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<HttpRequestMessage> PrepareProxyRequest()
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new ArgumentException("HttpContext cannot be null");

        var request = _httpContextAccessor.HttpContext.Request;
        string verb = request.Method.ToUpperInvariant();
        string path = request.Path.ToString().ToLowerInvariant();

        #region Remove leading and trailing slashes from path

        while (path.StartsWith('/')) path = path[1..];
        while (path.StartsWith("api/")) path = path[4..];
        string route = path.Split('/')[0];

        #endregion

        #region Check route is valid

        ProxyConfigService? microservice = _proxyConfig.Services.FirstOrDefault(
            service => service.Routes.Any(r => r == route));

        if (microservice == null) throw new InvalidRouteException(path);

        #endregion

        #region Prepare HTTP request

        var method = verb switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "PATCH" => HttpMethod.Patch,
            "DELETE" => HttpMethod.Delete,
            _ => throw new ArgumentException($"Unknown HTTP verb '{verb}'")
        };

        var query = request.QueryString.ToString();
        var baseUri = _configuration.GetValue<string>(microservice.BaseUri);

        var httpRequestMessage = new HttpRequestMessage(method, $"{baseUri}api/{path}{query}");

        if (request.Body != null)
        {
            using var streamReader = new StreamReader(request.Body);
            string body = await streamReader.ReadToEndAsync();
            httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (request.Headers.Keys.Contains("date-time-override"))
        {
            httpRequestMessage.Headers.Add("date-time-override", new string[] { request.Headers["date-time-override"][0] });
        }

        if (request.HasFormContentType)
        {
            var formData = request.Form;
            if (formData != null)
            {
                var multiContent = new MultipartFormDataContent();
                foreach (var key in formData.Keys)
                {
                    if (formData.TryGetValue(key, out var content))
                    {
                        var stringContent = new StringContent(content);
                        multiContent.Add(stringContent, key);
                    }
                }

                foreach (var file in formData.Files)
                {
                    var fileStreamContent = new StreamContent(file.OpenReadStream());
                    multiContent.Add(fileStreamContent, file.Name, file.FileName);
                }

                httpRequestMessage.Content = multiContent;
            }
        }

        if (_proxyConfig.UseCorrelationId)
        {
            httpRequestMessage.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        }

        return httpRequestMessage;

        #endregion
    }
}
