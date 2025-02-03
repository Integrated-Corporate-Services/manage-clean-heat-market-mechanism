using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IUserService
    {
        Task<HttpObjectResponse<List<ChmmUserDto>>> GetAll();
        Task<HttpObjectResponse<List<ChmmUserDto>>> GetAdminUsers();
        Task<HttpObjectResponse<ChmmUserDto>> GetAdminUser(Guid userId);
        Task<HttpObjectResponse<object>> InviteAdminUser(InviteAdminUserCommand command);
        Task<HttpObjectResponse<object>> ActivateAdminUser(ActivateAdminUserCommand command);
        Task<HttpObjectResponse<object>> DeactivateAdminUser(DeactivateAdminUserCommand command);
        Task<HttpObjectResponse<object>> UpdateUser(EditAdminUserCommand command);
        Task<HttpObjectResponse<List<ViewManufacturerUserDto>>> GetManufacturerUsers(Guid organisationId);
    }
}
