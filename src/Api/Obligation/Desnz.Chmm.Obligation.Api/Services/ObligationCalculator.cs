using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Common.Dtos;

namespace Desnz.Chmm.Obligation.Api.Services
{
    /// <summary>
    /// Enables obligation calculations
    /// </summary>
    public class ObligationCalculator : IObligationCalculator
    {
        private readonly ILogger<ObligationCalculator> _logger;

        public ObligationCalculator(ILogger<ObligationCalculator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Calculates obligations for a single boiler sales period
        /// </summary>
        /// <param name="gas"></param>
        /// <param name="oil"></param>
        /// <param name="gasBoilerSalesThreshold"></param>
        /// <param name="oilBoilerSalesthreshold"></param>
        /// <param name="targetRate"></param>
        /// <returns></returns>
        public int Calculate(int gas, int oil, int gasBoilerSalesThreshold, int oilBoilerSalesthreshold, decimal targetRate)
        {
            var gasAboveThreshold = Math.Max(0, gas - gasBoilerSalesThreshold);
            var oilAboveThreshold = Math.Max(0, oil - oilBoilerSalesthreshold);
            var fractional = (gasAboveThreshold + oilAboveThreshold) * targetRate / 100;
            return Convert.ToInt32(fractional);
        }

        /// <summary>
        /// Calculates obligations for the current quarter, factoring in previous quarters
        /// </summary>
        /// <param name="currentQuarterGas"></param>
        /// <param name="currentQuarterOil"></param>
        /// <param name="quarterlyBoilerSales"></param>
        /// <param name="quarterlyObligationsToDate"></param>
        /// <param name="gasBoilerSalesThreshold"></param>
        /// <param name="oilBoilerSalesthreshold"></param>
        /// <param name="targetRate"></param>
        /// <returns></returns>
        public int Calculate(int currentQuarterGas, int currentQuarterOil, IEnumerable<SalesNumbersDto> quarterlyBoilerSales, IEnumerable<int> quarterlyObligationsToDate, int gasBoilerSalesThreshold, int oilBoilerSalesthreshold, decimal targetRate)
        {
            var gasTotalToDate = quarterlyBoilerSales.Sum(x => x.Gas);
            var oilTotalToDate = quarterlyBoilerSales.Sum(x => x.Oil);
            var obligationTotalToDate = quarterlyObligationsToDate.Sum();

            var obligationTotal = Calculate(gasTotalToDate + currentQuarterGas, oilTotalToDate + currentQuarterOil, gasBoilerSalesThreshold, oilBoilerSalesthreshold, targetRate);

            int currentQuarterObligation = obligationTotal - obligationTotalToDate;

            return currentQuarterObligation;
        }

        public int CalculateCurrentObligationBalance(List<Transaction> transactions)
        {
            var totals = transactions.Where(t =>
                t.TransactionType == TransactionConstants.TransactionType.QuarterlySubmission ||
                t.TransactionType == TransactionConstants.TransactionType.AnnualSubmission ||
                t.TransactionType == TransactionConstants.TransactionType.AdminAdjustment ||
                t.TransactionType == TransactionConstants.TransactionType.BroughtFowardFromPreviousYear);
            var finalObligationsValue = totals.Sum(i => i.Obligation);

            var carriedOver = transactions.SingleOrDefault(t => t.TransactionType == TransactionConstants.TransactionType.CarryForwardToNextYear);
            var carriedOverValue = carriedOver?.Obligation ?? 0;

            var obligationsPaidOff = transactions.SingleOrDefault(t => t.TransactionType == TransactionConstants.TransactionType.Redeemed);
            var obligationsPaidOffValue = obligationsPaidOff?.Obligation ?? 0;

            return finalObligationsValue + carriedOverValue + obligationsPaidOffValue;
        }

        public ObligationSummaryDto GenerateSummary(List<Transaction> transactions)
        {
            var broughtForward = transactions.SingleOrDefault(t => t.TransactionType == TransactionConstants.TransactionType.BroughtFowardFromPreviousYear);
            var broughtForwardValue = broughtForward?.Obligation ?? 0;

            var carriedOver = transactions.SingleOrDefault(t => t.TransactionType == TransactionConstants.TransactionType.CarryForwardToNextYear);
            var carriedOverValue = carriedOver?.Obligation ?? 0;

            var adjustments = transactions.Where(t => t.TransactionType == TransactionConstants.TransactionType.AdminAdjustment);
            
            var generatedObligations = transactions.Where(t =>
                t.TransactionType == TransactionConstants.TransactionType.QuarterlySubmission ||
                t.TransactionType == TransactionConstants.TransactionType.AnnualSubmission);
            var generatedObligationsValue = generatedObligations.Sum(i => i.Obligation);

            var totals = transactions.Where(t =>
                t.TransactionType == TransactionConstants.TransactionType.QuarterlySubmission ||
                t.TransactionType == TransactionConstants.TransactionType.AnnualSubmission ||
                t.TransactionType == TransactionConstants.TransactionType.AdminAdjustment ||
                t.TransactionType == TransactionConstants.TransactionType.BroughtFowardFromPreviousYear);
            var finalObligationsValue = totals.Sum(i => i.Obligation);


            var obligationsPaidOff = transactions.SingleOrDefault(t => t.TransactionType == TransactionConstants.TransactionType.Redeemed);
            var obligationsPaidOffValue = obligationsPaidOff?.Obligation ?? 0;

            var adjustmentsRow = adjustments.Select(a => a.ToObligationSummaryItemDto());
            // Totals (give the date of the most recent transaction and sum all values;

            var remainingObligations = finalObligationsValue + carriedOverValue + obligationsPaidOffValue;

            return new ObligationSummaryDto(
                    generatedObligationsValue,
                    broughtForwardValue,
                    adjustmentsRow.ToList(),
                    finalObligationsValue,
                    carriedOverValue,
                    obligationsPaidOffValue,
                    remainingObligations);
        }
    }
}