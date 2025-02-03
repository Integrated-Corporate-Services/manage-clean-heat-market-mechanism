using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IMcsSynchronisationService
    {
        Task<HttpObjectResponse<List<InstallationRequestSummaryDto>>> GetInstallationRequests(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<List<CreditCalculationDto>>> GetInstallations(int manufacturerId, DateTime startDate, DateTime endDate);
        Task<HttpObjectResponse<DataPage<CreditCalculationDto>>> GetInstallations(Guid installationRequestId, int pageNumber, string? token = null);
    }
}
