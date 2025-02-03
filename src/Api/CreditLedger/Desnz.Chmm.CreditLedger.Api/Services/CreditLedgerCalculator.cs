using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Configuration.Common.ValueObjects;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Newtonsoft.Json;
using static Desnz.Chmm.Common.Constants.McsConstants;
using static Desnz.Chmm.CreditLedger.Constants.CreditLedgerConstants;

namespace Desnz.Chmm.CreditLedger.Api.Services
{
    /// <summary>
    /// Calculates the Installation credit
    /// </summary>
    public class CreditLedgerCalculator : ICreditLedgerCalculator
    {
        private readonly Dictionary<int, string> _technologyTypesEnum;
        private readonly Dictionary<int, string> _alternativeSystemFuelTypesEnum;

        private readonly ILogger<CreditLedgerCalculator> _logger;

        /// <summary>
        /// Calculates the Installation credit
        /// </summary>
        public CreditLedgerCalculator(
            ILogger<CreditLedgerCalculator> logger)
        {
            _logger = logger;

            _technologyTypesEnum = typeof(HeatPumpTechnologyTypes).ToDictionary();
            _alternativeSystemFuelTypesEnum = typeof(AlternativeSystemFuelTypes).ToDictionary();
        }
        protected CreditLedgerCalculator() { }

        /// <summary>
        /// Calculates the Installation credit
        /// </summary>
        /// <param name="installation"></param>
        /// <param name="schemeYearWeightings"></param>
        /// <returns></returns>
        public decimal Calculate(CreditCalculationDto installation, CreditWeightingDictionary schemeYearWeightings)
        {
            try
            {
                if (installation.HeatPumpProducts.Count == 0)
                {
                    _logger.LogWarning("No products when calculating credit for Installation Id {id}. Deafaulting credit to 0", installation.MidId);
                    return 0m;
                }

                if (!installation.IsNewBuildId.HasValue ||
                    !installation.TotalCapacity.HasValue ||
                    !installation.TechnologyTypeId.HasValue ||
                    (installation.TechnologyTypeId == HeatPumpTechnologyTypes.AirSourceHeatPump && !installation.AirTypeTechnologyId.HasValue) ||
                    !installation.RenewableSystemDesignId.HasValue ||
                    !installation.IsHybrid.HasValue)
                {
                    _logger.LogWarning("Insufficient data when calculating credit. Deafaulting credit to 0. Installation: {installation}", FormatInstallation(installation));
                    return 0.0m;
                }

                if (installation.IsNewBuildId == NewBuildOptions.Yes)
                {
                    return 0.0m;
                }

                if (installation.TotalCapacity > schemeYearWeightings.TotalCapacity)
                {
                    return 0.0m;
                }

                if (installation.RenewableSystemDesignId == RenewableSystemDesigns.AnotherPurposeOnly ||
                    installation.RenewableSystemDesignId == RenewableSystemDesigns.DhwAndAnotherPurpose ||
                    installation.RenewableSystemDesignId == RenewableSystemDesigns.DhwOnly)
                {
                    return 0.0m;
                }

                if ((installation.TechnologyTypeId == HeatPumpTechnologyTypes.AirSourceHeatPump || installation.TechnologyTypeId == HeatPumpTechnologyTypes.ExhaustAirHeatPump) &&
                    (installation.AirTypeTechnologyId == HeatPumpAirSourceTypes.AirToAirSource))
                {
                    return 0.0m;
                }

                var credit = schemeYearWeightings.TechnologyTypesWeightings[_technologyTypesEnum[installation.TechnologyTypeId.Value]] * installation.HeatPumpProducts.Count;

                if (credit == 0.0m)
                {
                    return credit;
                }


                if (!installation.IsHybrid.Value)
                {
                    return credit;
                }

                if (!AreHybridFieldsValid(installation))
                {
                    _logger.LogWarning("Insufficient data when calculating credit. Deafaulting credit to 0. Installation: {installation}", FormatInstallation(installation));
                    return 0.0m;
                }

                credit = schemeYearWeightings.AlternativeSystemFuelTypesWeightings[_alternativeSystemFuelTypesEnum[installation.AlternativeHeatingFuelId.Value]] * installation.HeatPumpProducts.Count;

                return credit;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calculating credit for Installation Id {id}. {error}", installation.MidId, JsonConvert.SerializeObject(ex));
                return 0m;
            }
        }

        private static bool AreHybridFieldsValid(CreditCalculationDto installation)
        {
            if (!installation.IsSystemSelectedAsMCSTechnology.HasValue)
                return false;

            if (!installation.AlternativeHeatingSystemId.HasValue)
                return false;

            if (!installation.AlternativeHeatingFuelId.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Generate the credit balance for a all given organisations
        /// </summary>
        /// <param name="organisationIds">Organisations to generate credit balances for</param>
        /// <param name="allOrganisationLicenceHolderCredits">Credits for all the given licence holders grouped by organisation</param>
        /// <param name="allTransactions">Transactions that apply to all organisations</param>
        /// <returns></returns>
        public List<OrganisationCreditBalanceDto> GenerateCreditBalances(
            List<Guid> organisationIds,
            List<OrganisationLicenceHolderCreditsDto> allOrganisationLicenceHolderCredits,
            List<Transaction> allTransactions)
        {
            var list = new List<OrganisationCreditBalanceDto>();
            foreach (var organisationId in organisationIds)
            {
                decimal balance = GenerateCreditBalance(
                    organisationId,
                    allOrganisationLicenceHolderCredits.Where(i => i.OrganisationId == organisationId).Sum(i => i.Credits),
                    allTransactions.Where(i => i.Entries.Any(i => i.OrganisationId == organisationId)).ToList());

                list.Add(new OrganisationCreditBalanceDto(organisationId, balance));
            }

            return list;
        }

        /// <summary>
        /// Generate the credit balance for a single organisation
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="organisationLicenceHolderCreditSum">Sum of all creditds assigned to the given licence holder ids</param>
        /// <param name="organisationTransactions">Transactions where at least one entry has a reference to the given organisation</param>
        /// <returns></returns>
        public decimal GenerateCreditBalance(
            Guid organisationId,
            decimal organisationLicenceHolderCreditSum,
            List<Transaction> organisationTransactions)
        {
            var transactionSum = organisationTransactions
                .Where(t =>
                            t.TransactionType != TransactionType.Redeemed &&
                            t.TransactionType != TransactionType.CarriedOverToNextYear
                       )
                .Sum(i => i.Entries.Where(e => e.OrganisationId == organisationId).Sum(e => e.Value));

            var balance = organisationLicenceHolderCreditSum + transactionSum;
            return balance;
        }

        /// <summary>
        /// Calculate the carry over value for carry over transactions
        /// </summary>
        /// <param name="credits">The current credit balance of an organisation</param>
        /// <param name="totalObligation">The organisations total obligation</param>
        /// <param name="creditCarryOverPercentage">The scheme year's carry over percentage</param>
        /// <returns></returns>
        public decimal CalculateCarryOver(decimal credits, decimal totalObligation, decimal creditCarryOverPercentage)
        {
            var creditsLessObligation = credits - totalObligation;
            var creditsTimesCarryOverPercentage = credits * creditCarryOverPercentage / 100;

            var unrounded = Math.Min(creditsLessObligation, creditsTimesCarryOverPercentage);

            return Math.Round(unrounded * 2, MidpointRounding.AwayFromZero) / 2;
        }

        /// <summary>
        /// Calculate the carry over value for newly linked licence holders
        /// </summary>
        /// <param name="creditsSum"></param>
        /// <param name="creditCarryOverPercentage"></param>
        /// <returns></returns>
        public virtual decimal CalculateNewLicenceHoldersCarryOver(decimal creditsSum, decimal creditCarryOverPercentage)
        {
            var unrounded = creditsSum * creditCarryOverPercentage / 100;
            return Math.Round(unrounded * 2, MidpointRounding.AwayFromZero) / 2;
        }

        /// <summary>
        /// Generates CreditLedger Summary
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="installationCredits"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        public CreditLedgerSummaryDto GenerateCreditLedgerSummary(
            Guid organisationId,
            IList<InstallationCredit> installationCredits,
            IList<Transaction> transactions
            )
        {
            // Generated Credits
            var creditsGeneratedByHeatPumps = installationCredits.Where(x => x.IsHybrid == false).Sum(x => x.Value);
            var creditsGeneratedByHybridHeatPumps = installationCredits.Where(x => x.IsHybrid == true).Sum(x => x.Value);

            // Transfers
            var transfers = transactions
                .Where(x => x.TransactionType == TransactionType.Transfer)
                .SelectMany(x => x.Entries)
                .Where(x => x.OrganisationId == organisationId);
            // In & Out
            var creditsTransferred = transfers.Sum(x => x.Value);

            // Carried Over
            var carriedOverFrom = transactions.Where(x => x.TransactionType == TransactionType.CarriedOverFromPreviousYear);
            var carriedOverFromPreviousYear = 0M;
            if (carriedOverFrom != null)
                carriedOverFromPreviousYear = carriedOverFrom.Sum(x => x.Entries.Single().Value);

            // Admin adjustments
            var creditsAmendedByAdministrator = transactions
                .Where(x => x.TransactionType == TransactionType.AdminAdjustment)
                .Select(x => new TransactionSummaryAdminAdjustmentDto(x.InitiatedBy, x.DateOfTransaction,
                    x.Entries.FirstOrDefault(y => y.OrganisationId == organisationId)?.Value ?? 0)).ToList();

            // Credit Sum
            var credits = creditsGeneratedByHeatPumps + creditsGeneratedByHybridHeatPumps;

            // Credit Balance
            var creditBalance = GenerateCreditBalance(organisationId, credits, transactions.ToList());

            // Redemption
            var redeemed = transactions
                .Where(x => x.TransactionType == TransactionType.Redeemed)
                .SelectMany(x => x.Entries)
                .SingleOrDefault(x => x.OrganisationId == organisationId);
            var redeemedCredits = redeemed?.Value ?? 0;

            // Carried Forward
            var carriedOverTo = transactions.Where(x => x.TransactionType == TransactionType.CarriedOverToNextYear);
            var carriedOverToNextYear = 0M;
            if (carriedOverTo != null)
                carriedOverToNextYear = carriedOverTo.Sum(x => x.Entries.Single().Value);

            var expired = -creditBalance - redeemedCredits - carriedOverToNextYear;

            var summary = new CreditLedgerSummaryDto(
                creditsGeneratedByHeatPumps,
                creditsGeneratedByHybridHeatPumps,
                creditsTransferred,
                carriedOverFromPreviousYear,
                creditsAmendedByAdministrator,
                creditBalance,
                redeemedCredits,
                carriedOverToNextYear,
                expired);

            return summary;
        }

        private static string FormatInstallation(CreditCalculationDto installation)
        {
            return string.Format("Id:{0}, IsNewBuildId:{1}, TotalCapacity:{2}, TechnologyTypeId:{3}, AirTypeTechnologyId:{4}, RenewableSystemDesignId:{5}, IsHybrid:{6}, AlternativeHeatingFuelId:{7}, AlternativeHeatingSystemId:{8}, IsSystemSelectedAsMCSTechnology:{9}",
                installation.MidId,
                installation.IsNewBuildId.HasValue ? installation.IsNewBuildId : "NULL",
                installation.TotalCapacity.HasValue ? installation.TotalCapacity : "NULL",
                installation.TechnologyTypeId.HasValue ? installation.TechnologyTypeId : "NULL",
                installation.AirTypeTechnologyId.HasValue ? installation.AirTypeTechnologyId : "NULL",
                installation.RenewableSystemDesignId.HasValue ? installation.RenewableSystemDesignId : "NULL",
                installation.IsHybrid.HasValue ? installation.IsHybrid : "NULL",
                installation.AlternativeHeatingFuelId.HasValue ? installation.AlternativeHeatingFuelId : "NULL",
                installation.AlternativeHeatingSystemId.HasValue ? installation.AlternativeHeatingSystemId : "NULL",
                installation.IsSystemSelectedAsMCSTechnology.HasValue ? installation.IsSystemSelectedAsMCSTechnology : "NULL");
        }
    }
}