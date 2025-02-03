using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Obligation.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    public class ObligationService : HttpServiceClient, IObligationService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        public ObligationService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        public async Task<HttpObjectResponse<ObligationSummaryDto>> GetObligationSummary(Guid organisationId, Guid schemeYearId, string? token = null)
        {
            return await HttpGetAsync<ObligationSummaryDto>($"api/obligation/organisation/{organisationId}/year/{schemeYearId}/summary", token: token);
        }

        /// <summary>
        /// Returns ALL ACTIVE organisations, and their obligation totals.
        /// Includes 0 if organisation has 0 boiler sales entered
        /// </summary>
        /// <param name="schemeYearId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<HttpObjectResponse<List<ObligationTotalDto>>> GetSchemeYearObligationTotals(Guid schemeYearId, string? token = null)
        {
            return await HttpGetAsync<List<ObligationTotalDto>>($"api/obligation/year/{schemeYearId}/totals", token: token);
        }

        public async Task<HttpObjectResponse<object>> SubmitAnnualObligation(CreateAnnualObligationCommand command)
        {
            return await HttpPostAsync<object>("api/obligation/annual", command);
        }

        public async Task<HttpObjectResponse<object>> SubmitQuarterlyObligation(CreateQuarterlyObligationCommand command)
        {
            return await HttpPostAsync<object>("api/obligation/quarterly", command);
        }

        public async Task<HttpObjectResponse<object>> RedeemObligations(RedeemObligationsCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/obligation/redeem", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> RollbackRedeemObligations(Guid schemeYearId, string? token = null)
        {
            var command = new RollbackRedeemObligationsCommand(schemeYearId);
            return await HttpPostAsync<object>("api/obligation/redeem/rollback", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> CarryForwardObligation(Guid schemeYearId, string? token = null)
        {
            var command = new CarryForwardObligationCommand(schemeYearId);
            return await HttpPostAsync<object>("api/obligation/carryforward", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> RollbackCarryForwardObligation(RollbackCarryForwardObligationCommand command, CancellationToken cancellationToken, string? token = null)
        {
            return await HttpPostAsync<object>("api/obligation/carryforward/rollback", command, token: token);
        }
    }
}
