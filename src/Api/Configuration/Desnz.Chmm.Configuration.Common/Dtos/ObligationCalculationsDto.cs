using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Configuration.Common.Dtos
{
    /// <summary>
    /// Configuration for a scheme year
    /// </summary>
    public class ObligationCalculationsDto
    {
        #region Properties

        /// <summary>
        /// The Obligation carry forward percentage cap
        /// </summary>
        public decimal PercentageCap { get; set; }

        /// <summary>
        /// The Obligation carry forward target multiplier
        /// </summary>
        public decimal TargetMultiplier { get; set; }

        /// <summary>
        /// The Obligation carry forward gas-specific credits cap
        /// </summary>
        public int GasCreditsCap { get; set; }

        /// <summary>
        /// The Obligation carry forward oil-specific credits cap
        /// </summary>
        public int OilCreditsCap { get; set; }

        /// <summary>
        /// Sales threshold for gas boilers
        /// </summary>
        public int GasBoilerSalesThreshold { get; set; }

        /// <summary>
        /// Sales threshold for oil boilers
        /// </summary>
        public int OilBoilerSalesThreshold { get; set; }

        /// <summary>
        /// Percentage rate for obligation generation
        /// </summary>
        public decimal TargetRate { get; set; }

        /// <summary>
        /// Credit carry over percentage
        /// </summary>
        public decimal CreditCarryOverPercentage { get; set; }

        public decimal AlternativeRenewableSystemFuelTypeWeightingValue { get; set; }
        public decimal AlternativeFossilFuelSystemFuelTypeWeightingValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constuctor
        /// </summary>
        public ObligationCalculationsDto() : base()
        {
        }
        #endregion
    }
}
