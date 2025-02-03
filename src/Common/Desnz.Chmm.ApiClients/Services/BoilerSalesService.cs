using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class BoilerSalesService : HttpServiceClient, IBoilerSalesService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public BoilerSalesService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<AnnualBoilerSalesDto>> GetAnnualBoilerSales(Guid organisationId, Guid schemeYearId)
        {
            return await HttpGetAsync<AnnualBoilerSalesDto>($"api/boilersales/organisation/{organisationId}/year/{schemeYearId}/annual");
        }

        public async Task<HttpObjectResponse<List<QuarterlyBoilerSalesDto>>> GetQuarterlyBoilerSales(Guid organisationId, Guid schemeYearId)
        {
            return await HttpGetAsync<List<QuarterlyBoilerSalesDto>>($"api/boilersales/organisation/{organisationId}/year/{schemeYearId}/quarters");
        }

        public async Task<HttpObjectResponse<BoilerSalesSummaryDto>> GetBoilerSalesSummary(Guid organisationId, Guid schemeYearId)
        {
            return await HttpGetAsync<BoilerSalesSummaryDto>($"api/boilersales/organisation/{organisationId}/year/{schemeYearId}/summary");
        }
        public async Task<HttpObjectResponse<List<BoilerSalesSummaryDto>>> GetAllBoilerSalesSummary(Guid schemeYearId)
        {
            return await HttpGetAsync<List<BoilerSalesSummaryDto>>($"api/boilersales/year/{schemeYearId}/summary");
        }
    }
}
