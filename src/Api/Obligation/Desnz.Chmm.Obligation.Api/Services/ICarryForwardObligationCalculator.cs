using Desnz.Chmm.Obligation.Api.Entities;

namespace Desnz.Chmm.Obligation.Api.Services
{
    public interface ICarryForwardObligationCalculator
    {
        /// <summary>
        /// Calculates carry forward obligation
        /// </summary>
        /// <param name="obligationTransactions"></param>
        /// <param name="currentYearId"></param>
        /// <param name="creditsGeneratedFromHeatPumpInstallations"></param>
        /// <param name="creditsCap"></param>
        /// <param name="percentageCap"></param>
        /// <param name="targetMultiplier"></param>
        /// 
        /// <returns></returns>
        int Calculate(IEnumerable<Transaction> obligationTransactions,
                      decimal creditsGeneratedFromHeatPumpInstallations,
                      int creditsCap,
                      decimal percentageCap,
                      decimal targetMultiplier);
    }
}