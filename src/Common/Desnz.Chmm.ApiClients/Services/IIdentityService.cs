using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IIdentityService
    {
        Task<HttpObjectResponse<string>> GetJwtToken(GetJwtTokenCommand command);
    }
}
