using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Desnz.Chmm.Web.Authentication;

public class ChmmOpenIdConnectBuilder
{
    private readonly OneLoginAuthConfig _oneLoginAuthConfig;

    public ChmmOpenIdConnectBuilder(OneLoginAuthConfig oneLoginAuthConfig)
    {
        _oneLoginAuthConfig = oneLoginAuthConfig;
    }

    public void ConfigureAuthentication(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }

    public void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.Cookie = new CookieBuilder
        {
            HttpOnly = true,
            Name = ChmmOpenIdConnectCookies.ChmmToken,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            SecurePolicy = CookieSecurePolicy.Always,
            MaxAge = TimeSpan.FromMinutes(_oneLoginAuthConfig.CookieMaxAge),
        };
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(_oneLoginAuthConfig.CookieMaxAge);
    }

    public void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
    {
        options.Authority = _oneLoginAuthConfig.Authority;
        options.ClientId = _oneLoginAuthConfig.ClientId;

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        options.EventsType = typeof(ChmmOpenIdConnectEvents);

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");

        options.MetadataAddress = $"{_oneLoginAuthConfig.Authority}/.well-known/openid-configuration";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = $"{_oneLoginAuthConfig.Authority}/",
            ValidAudience = _oneLoginAuthConfig.ClientId,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(_oneLoginAuthConfig.ClockSkewSeconds),
        };
    }
}
