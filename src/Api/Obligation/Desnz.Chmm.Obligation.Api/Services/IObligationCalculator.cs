using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Common.Dtos;

namespace Desnz.Chmm.Obligation.Api.Services
{
    public interface IObligationCalculator
    {
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
        int Calculate(int currentQuarterGas, int currentQuarterOil, IEnumerable<SalesNumbersDto> quarterlyBoilerSales, IEnumerable<int> quarterlyObligationsToDate, int gasBoilerSalesThreshold, int oilBoilerSalesthreshold, decimal targetRate);

        /// <summary>
        /// Calculates obligations for a single boiler sales period
        /// </summary>
        /// <param name="gas"></param>
        /// <param name="oil"></param>
        /// <param name="gasBoilerSalesThreshold"></param>
        /// <param name="oilBoilerSalesthreshold"></param>
        /// <param name="targetRate"></param>
        /// <returns></returns>
        int Calculate(int gas, int oil, int gasBoilerSalesThreshold, int oilBoilerSalesthreshold, decimal targetRate);

        /// <summary>
        /// Generate the obligation summary based on the values provided
        /// </summary>
        /// <param name="transactions">The list of transactions to build the summary from</param>
        /// <returns>The obligation summary</returns>
        ObligationSummaryDto GenerateSummary(List<Transaction> transactions);

        /// <summary>
        /// Calculate current obligation balance
        /// </summary>
        /// <param name="transactions">The list of transactions to calculate balance for</param>
        /// <returns>The obligation balance</returns>
        int CalculateCurrentObligationBalance(List<Transaction> transactions);
    }
}