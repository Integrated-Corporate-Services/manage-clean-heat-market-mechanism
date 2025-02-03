using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.YearEnd.Common.Commands;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class YearEndService : HttpServiceClient, IYearEndService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public YearEndService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }
        public async Task<HttpObjectResponse<object>> ProcessRedemption(ProcessRedemptionCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/yearend/redemption", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> RollbackRedemption(RollbackRedemptionCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/yearend/redemption/rollback", command, token: token);
        }
    }
}
