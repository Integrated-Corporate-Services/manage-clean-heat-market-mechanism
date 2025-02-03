using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Obligation.Api.Entities;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.Api.Services
{
    /// <summary>
    /// Enables obligation calculations
    /// </summary>
    public class CarryForwardObligationCalculator : ICarryForwardObligationCalculator
    {
        private readonly ILogger<CarryForwardObligationCalculator> _logger;

        public CarryForwardObligationCalculator(ILogger<CarryForwardObligationCalculator> logger)
        {
            _logger = logger;
        }

        public int Calculate(IEnumerable<Transaction> obligationTransactions,
                             decimal totalCreditsForManufacturer,
                             int maxObligationsToCarryForward, // 300 or 50
                             decimal percentageCap,            // 35%
                             decimal targetMultiplier          // 1x
            )
        {
            var totalObligations = obligationTransactions.Where(x => (x.TransactionType == TransactionType.AnnualSubmission ||
                                                                      x.TransactionType == TransactionType.AdminAdjustment ||
                                                                      x.TransactionType == TransactionType.BroughtFowardFromPreviousYear))
                                                         .Sum(x => x.Obligation);
            var shortfall = Math.Max(Convert.ToInt32(totalObligations - totalCreditsForManufacturer), 0);

            var obligationsGenerated = obligationTransactions.Where(x => (x.TransactionType == TransactionType.AnnualSubmission ||
                                                                          x.TransactionType == TransactionType.AdminAdjustment))
                                                             .Sum(x => x.Obligation);
            var currentYear35Percent = Convert.ToInt32(obligationsGenerated * (percentageCap / 100));

            if (shortfall == 0)
                return shortfall;

            var value = CalculateCarryForward(maxObligationsToCarryForward, targetMultiplier, shortfall, currentYear35Percent);

            return value;
        }

        private static int CalculateCarryForward(int maxObligationsToCarryForward, decimal targetMultiplier, int shortfall, int currentYear35Percent)
        {
            var multiplied35Percent = (currentYear35Percent * targetMultiplier).ToInt32();
            int val;
            if (maxObligationsToCarryForward > multiplied35Percent)
                val = maxObligationsToCarryForward;
            else
                val = multiplied35Percent;

            return Math.Min((shortfall * targetMultiplier).ToInt32(), val);
        }
    }
}