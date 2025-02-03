using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.YearEnd.Common.Commands;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IYearEndService
    {
        Task<HttpObjectResponse<object>> ProcessRedemption(ProcessRedemptionCommand command, string? token = null);
        Task<HttpObjectResponse<object>> RollbackRedemption(RollbackRedemptionCommand command, string? token = null);
    }
}
