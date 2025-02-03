using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Desnz.Chmm.Testing.Common
{
    public class TestAuthHandler : IAuthenticationHandler
    {
        static void SetAdmin(string email = "test@example.com")
        {
            Ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, "Regulatory Officer"),
                new Claim(ClaimTypes.Role, "Senior Technical Officer"),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            })), JwtBearerDefaults.AuthenticationScheme);
        }

        static TestAuthHandler()
        {
            SetAdmin();
        }

        public static AuthenticationTicket Ticket { get; set; }

        async Task<AuthenticateResult> IAuthenticationHandler.AuthenticateAsync()
        {
            return AuthenticateResult.Success(Ticket);
        }

        Task IAuthenticationHandler.ChallengeAsync(AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        Task IAuthenticationHandler.ForbidAsync(AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        Task IAuthenticationHandler.InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            return Task.CompletedTask;
        }
    }
}
