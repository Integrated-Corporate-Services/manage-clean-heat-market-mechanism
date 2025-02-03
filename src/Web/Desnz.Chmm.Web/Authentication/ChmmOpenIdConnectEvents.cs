using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Desnz.Chmm.Common.Constants.ConfigurationValueConstants;

namespace Desnz.Chmm.Web.Authentication;

public class ChmmOpenIdConnectEvents : OpenIdConnectEvents
{
    private readonly OneLoginAuthConfig _oneLoginAuthConfig;
    private readonly ChmmJwtAuthConfig _chmmJwtAuthConfig;

    private readonly CookieOptions _cookieOptions;
    private readonly IIdentityService _identityService;

    public ChmmOpenIdConnectEvents(IOptions<OneLoginAuthConfig> oneLoginAuthConfig, IOptions<ChmmJwtAuthConfig> chmmJwtAuthConfig, IIdentityService identityService)
    {
        _oneLoginAuthConfig = oneLoginAuthConfig.Value
            ?? throw new ArgumentNullException(GetErrorMessage(OneLoginAuth));
        _chmmJwtAuthConfig = chmmJwtAuthConfig.Value
            ?? throw new ArgumentNullException(GetErrorMessage(ChmmJwtAuth));
        _cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(_oneLoginAuthConfig.CookieMaxAge)
        };
        _identityService = identityService;
    }

    private JwtSecurityToken? GetJwtSecurityToken(SigningCredentials signingCredentials)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return new JwtSecurityToken(
            audience: $"{_oneLoginAuthConfig.Authority}/token",
            issuer: _oneLoginAuthConfig.ClientId,
            claims: new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _oneLoginAuthConfig.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.Integer64)
            },
            expires: DateTime.UtcNow.AddMinutes(3),
            signingCredentials: signingCredentials
        );
    }

    private IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_chmmJwtAuthConfig.SecurityKey);
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = _chmmJwtAuthConfig.Issuer,
            ValidAudience = _chmmJwtAuthConfig.Audience,
            ClockSkew = TimeSpan.FromSeconds(_chmmJwtAuthConfig.ClockSkewSeconds)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        return claimsPrincipal.Claims;
    }

    public override async Task RedirectToIdentityProvider(RedirectContext context)
    {
        await base.RedirectToIdentityProvider(context);
        context.ProtocolMessage.SetParameter("vtr", "[Cl]");
    }

    public override async Task TicketReceived(TicketReceivedContext context)
    {
        if (context.Properties is AuthenticationProperties properties)
        {
            var tokens = properties.GetTokens().ToList();

            var idToken = tokens.FirstOrDefault(t => t.Name == "id_token")?.Value;
            var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;

            var response = await _identityService.GetJwtToken(new GetJwtTokenCommand(idToken, accessToken));

            if (response.IsSuccessStatusCode && response.Result is not null)
            {
                var chmmToken = response.Result;
                var claims = GetClaimsFromToken(chmmToken);
                var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);

                if (context.Principal is ClaimsPrincipal principal)
                {
                    principal.AddIdentity(identity);
                    tokens.Add(new AuthenticationToken()
                    {
                        Name = IdentityConstants.Authentication.TokenName,
                        Value = chmmToken
                    });
                }
                properties.StoreTokens(tokens);
            }
        }
    }

    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(_oneLoginAuthConfig.RsaPrivateKey);
        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();

        var token = GetJwtSecurityToken(signingCredentials);
        var jwt = tokenHandler.WriteToken(token);

        if (context.TokenEndpointRequest is OpenIdConnectMessage tokenEndpointRequest)
        {
            tokenEndpointRequest.ClientAssertionType = ClientAssertionTypes.JwtBearer;
            tokenEndpointRequest.ClientAssertion = jwt;
        }

        if (context.Request.Query.TryGetValue("state", out var state))
        {
            context.Response.Cookies.Append(ChmmOpenIdConnectCookies.OneLoginState, state, _cookieOptions);
        }

        return Task.CompletedTask;
    }

    public override async Task RedirectToIdentityProviderForSignOut(RedirectContext context)
    {
        context.HttpContext.Response.Cookies.Delete(ChmmOpenIdConnectCookies.OneLoginState);
        context.HttpContext.Response.Cookies.Delete(ChmmOpenIdConnectCookies.ChmmToken);

        var props = context.ProtocolMessage;
        props.PostLogoutRedirectUri = _oneLoginAuthConfig.PostLogoutRedirectUri;

        var idToken = await context.HttpContext.GetTokenAsync("id_token");
        props.IdTokenHint = idToken;

        if (context.Request.Cookies.TryGetValue(ChmmOpenIdConnectCookies.OneLoginState, out var state))
        {
            props.State = state;
        }
    }
}
