using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Desnz.Chmm.ApiClients.Services
{
    public class OneLoginService : HttpServiceClient, IOneLoginService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public OneLoginService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys()
        {
            return await HttpGetAsync<JsonWebKeySet>(".well-known/jwks.json", false);
        }

        public async Task<HttpObjectResponse<OneLoginUserInfoDto>> GetUserInfo(string accessToken)
        {
            return await HttpGetAsync<OneLoginUserInfoDto>("userinfo ", token: accessToken);
        }
    }
}
