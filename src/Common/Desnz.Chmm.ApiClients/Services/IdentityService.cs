using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class IdentityService : HttpServiceClient, IIdentityService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<string>> GetJwtToken(GetJwtTokenCommand command)
        {
            return await HttpPostAsync<string>("api/identity/token", command, false);
        }
    }
}
