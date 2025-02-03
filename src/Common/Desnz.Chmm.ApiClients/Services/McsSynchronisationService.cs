using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class McsSynchronisationService : HttpServiceClient, IMcsSynchronisationService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public McsSynchronisationService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<List<CreditCalculationDto>>> GetInstallations(int manufacturerId, DateTime startDate, DateTime endDate)
        {
            var result =  await HttpGetAsync<List<CreditCalculationDto>>($"api/mcssynchronisation/organisation/{manufacturerId}/{startDate:yyyy-MM-dd}/{endDate:yyyy-MM-dd}");
            return result;
        }


        public async Task<HttpObjectResponse<DataPage<CreditCalculationDto>>> GetInstallations(Guid installationRequestId, int pageNumber, string? token = null)
        {
            var result = await HttpGetAsync<DataPage<CreditCalculationDto>>($"api/mcssynchronisation/installation-request/{installationRequestId}/{pageNumber}", token: token);
            return result;
        }

        public async Task<HttpObjectResponse<List<InstallationRequestSummaryDto>>> GetInstallationRequests(Guid schemeYearId, string? token = null)
        {
            var result = await HttpGetAsync<List<InstallationRequestSummaryDto>>($"api/mcssynchronisation/data/requests/year/{schemeYearId}", token: token);
            return result;
        }
    }
}
