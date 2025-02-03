using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IOneLoginService
    {
        Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys();
        Task<HttpObjectResponse<OneLoginUserInfoDto>> GetUserInfo(string accessToken);
    }
}
