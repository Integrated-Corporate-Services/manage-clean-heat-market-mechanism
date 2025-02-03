using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class RoleService : HttpServiceClient, IRoleService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public RoleService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<List<RoleDto>>> GetAdminRoles()
        {
            return await HttpGetAsync<List<RoleDto>>("api/identity/roles/admin");
        }
    }
}
