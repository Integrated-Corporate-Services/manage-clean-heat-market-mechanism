using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.ApiClients.Services
{
    /// <summary>
    /// Licence holder service allowing access to the Licence Holder services to other APIs
    /// </summary>
    public class CreditLedgerService : HttpServiceClient, ICreditLedgerService
    {
        /// <summary>
        /// WARNING: All Uris here MUST NOT have a leading /
        /// </summary>
        
        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="httpContextAccessor">Context Accessor</param>
        public CreditLedgerService(HttpClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
        }

        /// <summary>
        /// Create a licence holder
        /// </summary>
        /// <param name="command">Details of the licence holder to create</param>
        /// <returns>The Id of the created entity</returns>
        public async Task<HttpObjectResponse<object>> GenerateCredits(GenerateCreditsCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/creditledger/installation-request/generate-credits", command, token: token);
        }

        /// <summary>
        /// Returns the credit balance for all organisations
        /// </summary>
        /// <param name="schemeYearId">The scheme year to query</param>
        /// <returns>The balance for all the organisations</returns>
        public async Task<HttpObjectResponse<List<OrganisationCreditBalanceDto>>> GetAllCreditBalances(Guid schemeYearId, string? token = null)
        {
            return await HttpGetAsync<List<OrganisationCreditBalanceDto>>($"api/creditledger/year/{schemeYearId}/credit-balances", token: token);
        }

        public async Task<HttpObjectResponse<object>> RedeemCredits(RedeemCreditsCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/creditledger/redeem", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> RollbackRedeemCredits(Guid schemeYearId, string? token = null)
        {
            var command = new RollbackRedeemCreditsCommand(schemeYearId);
            return await HttpPostAsync<object>("api/creditledger/redeem/rollback", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> CarryOverCredit(Guid schemeYearId, string? token = null)
        {
            var command = new CarryOverCreditsCommand(schemeYearId);
            return await HttpPostAsync<object>("api/creditledger/carryover", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> CarryOverNewLicenceHolders(CarryOverNewLicenceHoldersCommand command, string? token = null)
        {
            return await HttpPostAsync<object>("api/creditledger/carryover-newlicenceholders", command, token: token);
        }

        public async Task<HttpObjectResponse<object>> RollbackCarryOverCredit(RollbackCarryOverCreditCommand command, CancellationToken cancellationToken, string? token = null)
        {
            return await HttpPostAsync<object>("api/creditledger/carryover/rollback", command, token: token);
        }

        public async Task<HttpObjectResponse<List<HeatPumpInstallationCreditsDto>>> GetInstallationCredits(DateOnly startDate, DateOnly endDate, string? token = null)
        {
            return await HttpGetAsync<List<HeatPumpInstallationCreditsDto>>($"api/creditledger/period/{startDate:yyyy-MM-dd}/{endDate:yyyy-MM-dd}", token: token);
        }
    }
}
