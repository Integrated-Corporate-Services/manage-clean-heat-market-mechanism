
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IBoilerSalesService
    {
        Task<HttpObjectResponse<AnnualBoilerSalesDto>> GetAnnualBoilerSales(Guid organisationId, Guid schemeYearId);
        Task<HttpObjectResponse<List<QuarterlyBoilerSalesDto>>> GetQuarterlyBoilerSales(Guid organisationId, Guid schemeYearId);

        Task<HttpObjectResponse<BoilerSalesSummaryDto>> GetBoilerSalesSummary(Guid organisationId, Guid schemeYearId);
        Task<HttpObjectResponse<List<BoilerSalesSummaryDto>>> GetAllBoilerSalesSummary(Guid schemeYearId);
    }
}
