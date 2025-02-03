using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Web.Proxy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Desnz.Chmm.Web.Controllers;

[Route("api")]
[ApiController]
public class ProxyController : ControllerBase
{
    private readonly IProxyService _proxyService;
    private readonly IProxyHttpServiceClient _proxyHttpServiceClient;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProxyController> _logger;

    public ProxyController(
        ILogger<ProxyController> logger, IProxyService proxyService, IProxyHttpServiceClient proxyHttpServiceClient, HttpClient httpClient)
    {
        _logger = logger;
        _proxyService = proxyService;
        _proxyHttpServiceClient = proxyHttpServiceClient;
        _httpClient = httpClient;
    }

    [Route("{*path}")]
    public async Task<IActionResult> Proxy(string path)
    {
        try
        {
            var request = await _proxyService.PrepareProxyRequest();

            var token = await HttpContext.GetTokenAsync(IdentityConstants.Authentication.TokenName);
            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            _logger.LogInformation("Sending HTTP request");
            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation("Receieved HTTP response");

            var headers = HttpContext.Response.Headers;
            foreach (var header in response.Headers)
            {
                var headerValue = string.Join(',', header.Value);
                if (!headers.ContainsKey(header.Key))
                {
                    headers.Add(header.Key, headerValue);
                }
            }

            foreach (var header in response.Content.Headers)
            {
                var headerValue = string.Join(',', header.Value);
                if (!headers.ContainsKey(header.Key))
                {
                    headers.Add(header.Key, headerValue);
                }
            }

            var hasContentTypeHeader = headers.TryGetValue("Content-Type", out var contentType);
            var hasContentDispositionHeader = headers.TryGetValue("Content-Disposition", out var contentDisposition);

            _logger.LogInformation($"HTTP Response Headers: {string.Join("; ", headers.Select(v => $"Key: {v.Key}; Value: {string.Join(", ", v.Value)}"))}");

            // If this is a file as an attachment, download it.
            if (hasContentDispositionHeader && contentDisposition.Any(i => !string.IsNullOrEmpty(i) && i.Contains("attachment")))
            {
                _logger.LogInformation("HTTP response was file attachment");
                var stream = response.Content.ReadAsStream();
                stream.Seek(0, SeekOrigin.Begin);

                return Responses.File(stream, contentType);
            }

            _logger.LogInformation("HTTP response was json blob");
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"HTTP response blob ({response.StatusCode}): {content}");
            var result = new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = content,
                ContentType = hasContentTypeHeader ? contentType : "application/json"
            };
            return result;
        }
        catch (InvalidRouteException ex)
        {
            _logger.LogCritical($"Invalid Route in Proxy Controller: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Failure in Proxy Controller: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
