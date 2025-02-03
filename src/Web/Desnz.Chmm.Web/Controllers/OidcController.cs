using Amazon.S3.Model;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Desnz.Chmm.Web.Controllers
{
    [ApiController]
    [Route("oidc")]
    public class OidcController : ControllerBase
    {
        private readonly ILogger<OidcController> _logger;
        private readonly IIdentityService _identityService;

        public OidcController(ILogger<OidcController> logger, IIdentityService identityService)
        {
            _logger = logger;
            _identityService = identityService;
        }

        [HttpGet("login")]
        public ActionResult Login(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase : "/";
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase : "/";
            }

            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("whoami")]
        public  ActionResult<UserDetails?> WhoAmI()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var email = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
                var roles = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
                var status = User.Claims.FirstOrDefault(x => x.Type == IdentityConstants.Claims.Status)?.Value;
                var organisationId = User.Claims.FirstOrDefault(x => x.Type == IdentityConstants.Claims.OrganisationId)?.Value;

                var exp = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
                DateTime? expDate = null;
                if (exp != null && long.TryParse(exp, out var seconds))
                {
                    expDate = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
                }

                return new UserDetails(email, roles, status, organisationId, expDate);
            }
            else
            {
                return Ok();
            }
        }

        public record UserDetails(string? Email = null, List<string>? Roles = null, string? Status = null, string? OrganisationId = null, DateTime? exp = null);
    }
}
