using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Desnz.Chmm.Testing.Common;

public class IntegrationTestsBase<TLocalEntryPoint> : IDisposable where TLocalEntryPoint : class
{
    private readonly Application _application;
    protected readonly HttpClient _client;

    private static readonly IDictionary<string, Action<IWebHostBuilder>?> _configureHostBuilders;

    #region Constructors and deconstructors

    static IntegrationTestsBase()
    {
        _configureHostBuilders = new Dictionary<string, Action<IWebHostBuilder>?>();
    }

    public IntegrationTestsBase() : this(null) { }

    public IntegrationTestsBase(Action<IWebHostBuilder>? configureHostBuilder)
    {
        string entryPoint = typeof(TLocalEntryPoint).FullName;
        if (_configureHostBuilders.ContainsKey(entryPoint))
            _configureHostBuilders.Remove(entryPoint);
        _configureHostBuilders.Add(entryPoint, configureHostBuilder);

        _application = new Application();
        _client = _application.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme);
    }

    public void Dispose()
    {
        _client.Dispose();
        _application.Dispose();
    }

    #endregion

    private class Application : WebApplicationFactory<TLocalEntryPoint>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if (_configureHostBuilders.TryGetValue(typeof(TLocalEntryPoint).FullName, out var configure))
            {
                configure?.Invoke(builder);
            }

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    var authSchemeBuilder = new AuthenticationSchemeBuilder(JwtBearerDefaults.AuthenticationScheme)
                    {
                        HandlerType = typeof(TestAuthHandler)
                    };

                    options.Schemes.First(s => s.Name == JwtBearerDefaults.AuthenticationScheme).HandlerType = typeof(TestAuthHandler);
                    options.SchemeMap[JwtBearerDefaults.AuthenticationScheme] = authSchemeBuilder;
                });
            });

            base.ConfigureWebHost(builder);
        }
    }

    #region Helper functions

    #region Get

    protected async Task CheckGet<T>(string uri, int statusCode, T expected)
    {
        await CheckGet<T>(uri, statusCode, actual => actual.Should().BeEquivalentTo(expected));
    }

    protected async Task CheckGet<T>(string uri, int statusCode, Action<T> check)
    {
        // Act
        var response = await _client.GetAsync(uri);
        var actual = await ParseResponse<T>(response);

        // Assert
        ((int)response.StatusCode).Should().Be(statusCode);
        check(actual);
    }

    #endregion

    #region Post

    protected async Task CheckPost<T>(string uri, object data, int statusCode, T expected)
    {
        await CheckPost<T>(uri, data, statusCode, actual => actual.Should().BeEquivalentTo(expected));
    }

    protected async Task CheckPost<T>(string uri, object data, int statusCode, Action<T> check)
    {
        // Act
        var content = JsonContent.Create(data);
        var response = await _client.PostAsync(uri, content);
        var actual = await ParseResponse<T>(response);

        // Assert
        ((int)response.StatusCode).Should().Be(statusCode);
        check(actual);
    }

    #endregion

    protected async Task<TResponse> ParseResponse<TResponse>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(json);
    }

    #endregion
}
