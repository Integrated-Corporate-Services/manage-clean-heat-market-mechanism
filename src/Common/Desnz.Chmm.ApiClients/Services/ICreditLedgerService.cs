using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Common.Dtos;

namespace Desnz.Chmm.ApiClients.Services
{
    /// <summary>
    /// Credit ledger service allowing access to the Credit Ledger services to other APIs
    /// </summary>
    public interface ICreditLedgerService
    {
        /// <summary>
        /// Generates licence holder credits
        /// </summary>
        /// <param name="command">Details of the licence holder to create</param>
        /// <returns>The Id of the created entity</returns>
        Task<HttpObjectResponse<object>> GenerateCredits(GenerateCreditsCommand command, string? token = null);

        /// <summary>
        /// Returns the credit balance for all organisations
        /// </summary>
        /// <param name="schemeYearId">The scheme year to query</param>
        /// <returns>The balance for all the organisations</returns>
        Task<HttpObjectResponse<List<OrganisationCreditBalanceDto>>> GetAllCreditBalances(Guid schemeYearId, string? token = null);

        /// <summary>
        /// Log a collection of credit redemptions for a given year
        /// </summary>
        /// <param name="command">The redemptions to be appled to the current year</param>
        /// <param name="token">Authentication token</param>
        /// <returns></returns>
        Task<HttpObjectResponse<object>> RedeemCredits(RedeemCreditsCommand command, string? token = null);

        /// <summary>
        /// Trigger the carry forward credit command
        /// </summary>
        /// <param name="schemeYearId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<HttpObjectResponse<object>> CarryOverCredit(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<object>> RollbackRedeemCredits(Guid schemeYearId, string? token = null);
        Task<HttpObjectResponse<object>> RollbackCarryOverCredit(RollbackCarryOverCreditCommand command, CancellationToken cancellationToken, string? token = null);
        Task<HttpObjectResponse<List<HeatPumpInstallationCreditsDto>>> GetInstallationCredits(DateOnly startDate, DateOnly endDate, string? token = null);
        Task<HttpObjectResponse<object>> CarryOverNewLicenceHolders(CarryOverNewLicenceHoldersCommand command, string? token = null);
    }
}
