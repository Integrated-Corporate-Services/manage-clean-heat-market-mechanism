using Desnz.Chmm.Configuration.Common.ValueObjects;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.CreditLedger.Api.Services
{
    /// <summary>
    /// Provides credit calculation  functionality
    /// </summary>
    public interface ICreditLedgerCalculator
    {
        /// <summary>
        /// Calculates the installation credits
        /// </summary>
        /// <param name="installation"></param>
        /// <param name="schemeYearWeightings"></param>
        /// <returns></returns>
        decimal Calculate(CreditCalculationDto installation, CreditWeightingDictionary schemeYearWeightings);

        /// <summary>
        /// Generate the credit balance for a all given organisations
        /// </summary>
        /// <param name="organisationIds">Organisations to generate credit balances for</param>
        /// <param name="allOrganisationLicenceHolderCredits">Credits for all the given licence holders grouped by organisation</param>
        /// <param name="allTransactions">Transactions that apply to all organisations</param>
        /// <returns></returns>
        List<OrganisationCreditBalanceDto> GenerateCreditBalances(
            List<Guid> organisationIds,
            List<OrganisationLicenceHolderCreditsDto> allOrganisationLicenceHolderCredits,
            List<Transaction> allTransactions);

        /// <summary>
        /// Generate the credit balance for a single organisation
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="organisationLicenceHolderCreditSum">All creditds assigned to the given licence holder ids</param>
        /// <param name="organisationTransactions">Transactions where at least one entry has a reference to the given organisation</param>
        /// <returns></returns>
        decimal GenerateCreditBalance(
            Guid organisationId,
            decimal organisationLicenceHolderCreditSum,
            List<Transaction> organisationTransactions);

        decimal CalculateCarryOver(decimal credits, decimal totalObligation, decimal creditCarryOverPercentage);

        CreditLedgerSummaryDto GenerateCreditLedgerSummary(
            Guid organisationId,
            IList<InstallationCredit> installationCredits,
            IList<Transaction> transactions);
        
        decimal CalculateNewLicenceHoldersCarryOver(decimal creditsSum, decimal creditCarryOverPercentage);
    }
}