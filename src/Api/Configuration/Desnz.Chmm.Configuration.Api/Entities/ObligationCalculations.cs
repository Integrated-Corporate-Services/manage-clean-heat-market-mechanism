using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Configuration.Common.Commands;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Configuration for a scheme year
    /// </summary>
    public class ObligationCalculations : Entity
    {
        #region Properties

        /// <summary>
        /// Id of the scheme year this quarter is assocated with
        /// </summary>
        public Guid SchemeYearId { get; private set; }

        /// <summary>
        /// The Scheme Year the obligation calculations are for
        /// </summary>
        public SchemeYear SchemeYear { get; private set; }

        /// <summary>
        /// The Obligation carry forward percentage cap
        /// </summary>
        public decimal PercentageCap { get; private set; }

        /// <summary>
        /// The Obligation carry forward target multiplier
        /// </summary>
        public decimal TargetMultiplier { get; private set; }

        /// <summary>
        /// The Obligation carry forward gas-specific credits cap
        /// </summary>
        public int GasCreditsCap { get; private set; }

        /// <summary>
        /// The Obligation carry forward oil-specific credits cap
        /// </summary>
        public int OilCreditsCap { get; private set; }

        /// <summary>
        /// Sales threshold for gas boilers
        /// </summary>
        public int GasBoilerSalesThreshold { get; private set; }

        /// <summary>
        /// Sales threshold for oil boilers
        /// </summary>
        public int OilBoilerSalesThreshold { get; private set; }

        /// <summary>
        /// Percentage rate for obligation generation
        /// </summary>
        public decimal TargetRate { get; private set; }

        /// <summary>
        /// Credit carry over percentage
        /// </summary>
        public decimal CreditCarryOverPercentage { get; private set; }

        #endregion

        #region Constructors

        public ObligationCalculations(decimal percentageCap,
                                      decimal targetMultiplier,
                                      int gasCreditsCap,
                                      int oilCreditsCap,
                                      int gasBoilerSalesThreshold,
                                      int oilBoilerSalesThreshold,
                                      decimal targetRate,
                                      decimal creditCarryOverPercentage)
        {
            PercentageCap = percentageCap;
            TargetMultiplier = targetMultiplier;
            GasCreditsCap = gasCreditsCap;
            OilCreditsCap = oilCreditsCap;
            GasBoilerSalesThreshold = gasBoilerSalesThreshold;
            OilBoilerSalesThreshold = oilBoilerSalesThreshold;
            TargetRate = targetRate;
            CreditCarryOverPercentage = creditCarryOverPercentage;
        }

        /// <summary>
        /// Default constuctor
        /// </summary>
        protected ObligationCalculations() : base()
        {
        }

        #endregion

        #region Behaviours

        /// <summary>
        /// Edit properties using <see cref="UpdateSchemeYearConfigurationCommand" />
        /// </summary>
        /// <param name="command">Command with new configuration</param>
        public void Update(decimal targetRate, decimal percentageCap, decimal targetMultiplier, decimal creditCarryOverPercentage, int gasBoilerSalesThreshold, int oilBoilerSalesThreshold)
        {
            TargetRate = targetRate;
            PercentageCap = percentageCap;
            TargetMultiplier = targetMultiplier;
            CreditCarryOverPercentage = creditCarryOverPercentage;
            GasBoilerSalesThreshold = gasBoilerSalesThreshold;
            OilBoilerSalesThreshold = oilBoilerSalesThreshold;
        }

        #endregion
    }
}
