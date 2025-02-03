using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Obligation.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.ApiClients.Services
{
    public interface IObligationService
    {
        Task<HttpObjectResponse<object>> SubmitAnnualObligation(CreateAnnualObligationCommand command);
        Task<HttpObjectResponse<object>> SubmitQuarterlyObligation(CreateQuarterlyObligationCommand command);
        Task<HttpObjectResponse<ObligationSummaryDto>> GetObligationSummary(Guid organisationId, Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<List<ObligationTotalDto>>> GetSchemeYearObligationTotals(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<object>> RedeemObligations(RedeemObligationsCommand command, string? token = null);
        Task<HttpObjectResponse<object>> RollbackRedeemObligations(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<object>> CarryForwardObligation(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<object>> RollbackCarryForwardObligation(RollbackCarryForwardObligationCommand command, CancellationToken cancellationToken, string? token = null);
    }
}
