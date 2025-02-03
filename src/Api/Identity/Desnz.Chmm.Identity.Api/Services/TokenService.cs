using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Identity.Api.Exceptions;
using static Desnz.Chmm.Common.Constants.ConfigurationValueConstants;
using Desnz.Chmm.Identity.Api.Entities;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Identity.Api.Constants.UsersConstants;

namespace Desnz.Chmm.Identity.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly OneLoginAuthConfig _oneLoginAuthConfig;
        private readonly ChmmJwtAuthConfig _chmmJwtAuthConfig;
        private readonly IOneLoginService _oneLoginService;

        public TokenService(IOptions<OneLoginAuthConfig> oneLoginAuthConfig, IOptions<ChmmJwtAuthConfig> chmmJwtAuthConfig, IOneLoginService oneLoginService)
        {
            _oneLoginAuthConfig = oneLoginAuthConfig.Value
                ?? throw new InvalidOperationException(GetErrorMessage(OneLoginAuth));
            _chmmJwtAuthConfig = chmmJwtAuthConfig.Value
                ?? throw new InvalidOperationException(GetErrorMessage(ChmmJwtAuth));
            _oneLoginService = oneLoginService;
        }

        public async Task ValidateTokenAsync(string idToken, CancellationToken cancellationToken)
        {
            var response = await _oneLoginService.GetSigningKeys();
            if (!response.IsSuccessStatusCode)
            {
                response.Problem.ThrowException();
            }

            var keys = (response.Result?.Keys) ?? throw new ChmmIdentityException("GOV.UK One Login returned an empty key set");

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = $"{_oneLoginAuthConfig.Authority}/",
                ValidAudience = _oneLoginAuthConfig.ClientId,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => keys,
                ClockSkew = TimeSpan.FromSeconds(_oneLoginAuthConfig.ClockSkewSeconds)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(idToken, tokenValidationParameters, out SecurityToken validatedToken);
        }

        public string GenerateJwtToken(string email, ChmmUser? user, string? apiKey = null)
        {
            var key = Encoding.UTF8.GetBytes(_chmmJwtAuthConfig.SecurityKey);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            IEnumerable<Claim> claims;
            if (apiKey != null)
            {
                claims = GetApiClaims(apiKey);
            }
            else
            {
                claims = user != null ? GetClaims(user) : GetDefaultClaims(email);
            }

            var token = new JwtSecurityToken(
                issuer: _chmmJwtAuthConfig.Issuer,
                audience: _chmmJwtAuthConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_chmmJwtAuthConfig.MinutesUntilExpiration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private IEnumerable<Claim> GetDefaultClaims(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Sid, string.Empty)
            };
            return claims;
        }

        private IEnumerable<Claim> GetClaims(ChmmUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString())
            };

            if (user.OrganisationId.HasValue)
            {
                claims.Add(new Claim(Claims.OrganisationId, user.OrganisationId.Value.ToString()));
            }

            if (user.Status == Status.Active)
            {
                claims.AddRange(user.ChmmRoles.Select(r => new Claim(ClaimTypes.Role, r.Name)));
            }

            claims.Add(new Claim(Claims.Status, user.Status));

            return claims;
        }

        private List<Claim> GetApiClaims(string apiKey)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, apiKey),
                new Claim(ClaimTypes.Role, Roles.ApiRole),
            };
            return claims;
        }
    }
}
