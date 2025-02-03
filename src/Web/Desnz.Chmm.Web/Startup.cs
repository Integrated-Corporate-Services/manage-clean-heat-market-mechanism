using Desnz.Chmm.Web.Authentication;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Web.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Web;

public class Startup
{
    private readonly WebApplicationBuilder _builder;

    public Startup(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public void Run()
    {
        var services = _builder.Services;
        var config = _builder.Configuration;
#if !DEBUG
        config.AddAmazonSecretsManager();
#endif
        var oneLoginAuthConfigSection = config.GetSection(ConfigurationValueConstants.OneLoginAuth);
        services.Configure<OneLoginAuthConfig>(oneLoginAuthConfigSection);
        services.Configure<ChmmJwtAuthConfig>(config.GetSection(ConfigurationValueConstants.ChmmJwtAuth));
        services.Configure<GoogleAnalyticsConfig>(config.GetSection(ConfigurationValueConstants.GoogleAnalytics));

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
        });

        services.AddCors(options =>
        {
            options.AddPolicy(name: "_chmmAllowOrigins",
                policy =>
                {
                    policy.WithOrigins("https://oidc.integration.account.gov.uk");
                });
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole();

            // Enable OpenID Connect logging
            loggingBuilder.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
        });

        #region Authentication

        var oneLoginAuthConfig = oneLoginAuthConfigSection.Get<OneLoginAuthConfig>()
            ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.OneLoginAuth));

        var chmmOpenIdConnectBuilder = new ChmmOpenIdConnectBuilder(oneLoginAuthConfig);
        services
            .AddTransient(typeof(ChmmOpenIdConnectEvents))
            .AddAuthentication(chmmOpenIdConnectBuilder.ConfigureAuthentication)
            .AddCookie(chmmOpenIdConnectBuilder.ConfigureCookie)
            .AddOpenIdConnect(chmmOpenIdConnectBuilder.ConfigureOpenIdConnectOptions);

        #endregion

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddProxyServices(config);
        services.AddHttpClientServices(config);

        services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
        services.AddRazorPages();
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        var app = _builder.Build();

        app.UseForwardedHeaders();
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();

        app.UseXsrfToken();
        app.UseSecurityHeaders();
        app.UseGoogleAnalytics();

        app.UseCors("_chmmAllowOrigins");
        app.UseAuthorization();
        app.MapControllers();

        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}
