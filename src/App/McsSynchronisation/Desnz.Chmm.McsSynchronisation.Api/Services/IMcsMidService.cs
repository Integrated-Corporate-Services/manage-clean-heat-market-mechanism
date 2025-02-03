
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.McsSynchronisation.Api.Services
{
    public interface IMcsMidService
    {
        Task<HttpObjectResponse<McsInstallationsDto>> GetHeatPumpInstallations(GetMcsInstallationsDto content);
    }
}
