
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Dtos;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IRoleService
    {
        Task<HttpObjectResponse<List<RoleDto>>> GetAdminRoles();
    }
}
