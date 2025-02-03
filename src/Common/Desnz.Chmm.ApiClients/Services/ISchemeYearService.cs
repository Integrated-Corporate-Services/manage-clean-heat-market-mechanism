using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Commands;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.ApiClients.Services;

public interface ISchemeYearService
{
    Task<HttpObjectResponse<SchemeYearDto>> GetNextSchemeYear(Guid schemeYearId, CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<SchemeYearDto>> GetSchemeYear(Guid schemeYearId, CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<SchemeYearDto>> GetFirstSchemeYear(string? token = null);
    Task<HttpObjectResponse<SchemeYearDto>> GetCurrentSchemeYear(CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<SchemeYearDto>> GetCurrentSchemeYearBySurrenderDay(CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<SchemeYearQuarterDto>> GetCurrentSchemeYearQuarter(CancellationToken cancellationToken);
    Task<HttpObjectResponse<List<SchemeYearDto>>> GetAllSchemeYears(CancellationToken cancellationToken);
    Task<HttpObjectResponse<CreditWeightingsDto>> GetCreditWeightings(Guid schemeYeraId, CancellationToken cancellationToken);
    Task<HttpObjectResponse<List<CreditWeightingsDto>>> GetAllCreditWeightings(CancellationToken cancellationToken);
    Task<HttpObjectResponse<object>> GenerateNextYearsSchemea(GenerateNextYearsSchemeaCommand command, CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<object>> RollbackGenerateNextYearsSchemea(RollbackGenerateNextYearsSchemeCommand command, CancellationToken cancellationToken, string? token = null);
    Task<HttpObjectResponse<ObligationCalculationsDto>> GetObligationCalculations(Guid schemeYearId, CancellationToken cancellationToken);
}
