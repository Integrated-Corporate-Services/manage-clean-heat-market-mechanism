using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Commands;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services;

/// <summary>
/// TODO: Implement dynamic scheme year based on configuration service
/// </summary>
public class SchemeYearService : HttpServiceClient, ISchemeYearService
{
    /// <summary>
    /// WARNING: All Uris here MUST NOT have a leading /
    /// </summary>
    public SchemeYearService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
    {
    }

    public async Task<HttpObjectResponse<List<SchemeYearDto>>> GetAllSchemeYears(CancellationToken cancellationToken)
    {
        return await HttpGetAsync<List<SchemeYearDto>>("api/configuration/schemeyear/all");
    }

    public async Task<HttpObjectResponse<SchemeYearDto>> GetFirstSchemeYear(string? token = null)
    {
        return await HttpGetAsync<SchemeYearDto>("api/configuration/schemeyear/first", token: token);
    }

    public async Task<HttpObjectResponse<SchemeYearDto>> GetCurrentSchemeYear(CancellationToken cancellationToken, string? token = null)
    {
        return await HttpGetAsync<SchemeYearDto> ("api/configuration/schemeyear/current", token: token);
    }

    public async Task<HttpObjectResponse<SchemeYearDto>> GetCurrentSchemeYearBySurrenderDay(CancellationToken cancellationToken, string? token = null)
    {
        return await HttpGetAsync<SchemeYearDto> ("api/configuration/schemeyear/current/surrender-day", token: token);
    }

    public async Task<HttpObjectResponse<SchemeYearQuarterDto>> GetCurrentSchemeYearQuarter(CancellationToken cancellationToken)
    {
        return await HttpGetAsync<SchemeYearQuarterDto>("api/configuration/schemeyear/quarter/current");
    }

    public async Task<HttpObjectResponse<SchemeYearDto>> GetSchemeYear(Guid schemeYearId, CancellationToken cancellationToken, string? token = null)
    {
        return await HttpGetAsync<SchemeYearDto>($"api/configuration/schemeyear/{schemeYearId}", token: token);
    }

    public async Task<HttpObjectResponse<SchemeYearDto>> GetNextSchemeYear(Guid schemeYearId, CancellationToken cancellationToken, string? token = null)
    {
        return await HttpGetAsync<SchemeYearDto>($"api/configuration/schemeyear/{schemeYearId}/next", token: token);
    }

    public async Task<HttpObjectResponse<CreditWeightingsDto>> GetCreditWeightings(Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await HttpGetAsync<CreditWeightingsDto>($"api/configuration/creditweighting/{schemeYearId}");
    }

    public async Task<HttpObjectResponse<List<CreditWeightingsDto>>> GetAllCreditWeightings(CancellationToken cancellationToken)
    {
        return await HttpGetAsync<List<CreditWeightingsDto>>("api/configuration/creditweighting/all");
    }

    public async Task<HttpObjectResponse<object>> GenerateNextYearsSchemea(GenerateNextYearsSchemeaCommand command, CancellationToken cancellationToken, string? token = null)
    {
        return await HttpPostAsync<object>($"api/configuration/schemeyear", command, token: token);
    }

    public async Task<HttpObjectResponse<object>> RollbackGenerateNextYearsSchemea(RollbackGenerateNextYearsSchemeCommand command, CancellationToken cancellationToken, string? token = null)
    {
        return await HttpPostAsync<object>($"api/configuration/schemeyear/rollback", command, token: token);
    }

    public async Task<HttpObjectResponse<ObligationCalculationsDto>> GetObligationCalculations(Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await HttpGetAsync<ObligationCalculationsDto>($"api/configuration/obligationcalculations/{schemeYearId}");
    }
}