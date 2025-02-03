using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Testing.Common
{
    public class TestClaimsBase
    {
        protected ClaimsPrincipal GetMockManufacturerUser(Guid userId, Guid organisationId)
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@test"),
                new Claim(ClaimTypes.Role, Roles.Manufacturer),
                new Claim(Claims.OrganisationId, organisationId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()),
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return httpContext.User;
        }

        protected ClaimsPrincipal GetMockAdminUser(Guid userId)
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@test"),
                new Claim(ClaimTypes.Role, Roles.PrincipalTechnicalOfficer),
                new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()),
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return httpContext.User;
        }
    }
}
