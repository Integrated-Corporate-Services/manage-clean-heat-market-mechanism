using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class UserService : HttpServiceClient, IUserService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public UserService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<List<ChmmUserDto>>> GetAll()
        {
            return await HttpGetAsync<List<ChmmUserDto>>("api/identity/users/admin");
        }

        public async Task<HttpObjectResponse<List<ChmmUserDto>>> GetAdminUsers()
        {
            return await HttpGetAsync<List<ChmmUserDto>>("api/identity/users/admin");
        }

        public async Task<HttpObjectResponse<List<ViewManufacturerUserDto>>> GetManufacturerUsers(Guid organisationId)
        {
            return await HttpGetAsync<List<ViewManufacturerUserDto>>($"api/identity/users/manufacturer/{organisationId}");
        }

        public async Task<HttpObjectResponse<ChmmUserDto>> GetAdminUser(Guid userId)
        {
            return await HttpGetAsync<ChmmUserDto>($"api/identity/users/admin/{userId}");
        }

        public async Task<HttpObjectResponse<object>> InviteAdminUser(InviteAdminUserCommand command)
        {
            return await HttpPostAsync<object>("api/identity/users/admin", command);
        }

        public async Task<HttpObjectResponse<object>> ActivateAdminUser(ActivateAdminUserCommand command)
        {
            return await HttpPutAsync<object>("api/identity/users/admin/activate", command);
        }

        public async Task<HttpObjectResponse<object>> DeactivateAdminUser(DeactivateAdminUserCommand command)
        {
            return await HttpPutAsync<object>("api/identity/users/admin/deactivate", command);
        }

        public async Task<HttpObjectResponse<object>> UpdateUser(EditAdminUserCommand command)
        {
            return await HttpPutAsync<object>("api/identity/users/admin", command);
        }
    }
}
