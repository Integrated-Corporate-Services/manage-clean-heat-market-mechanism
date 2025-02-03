using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Weightings for alternative system fuel types
    /// </summary>
    public class AlternativeSystemFuelTypeWeighting : Entity
    {

        /// <summary>
        /// Fuel type code
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Id of the associated credit weighting
        /// </summary>
        public Guid CreditWeightingId { get; private set; }

        /// <summary>
        /// Link to the associated credit weighting
        /// </summary>
        public CreditWeighting CreditWeighting { get; private set; }

        /// <summary>
        /// Id of the value
        /// </summary>
        public Guid AlternativeSystemFuelTypeWeightingValueId { get; private set; }

        /// <summary>
        /// Link to the value
        /// </summary>
        public AlternativeSystemFuelTypeWeightingValue AlternativeSystemFuelTypeWeightingValue { get; private set; }

        protected AlternativeSystemFuelTypeWeighting() : base()
        {
            
        }
        public AlternativeSystemFuelTypeWeighting(string code, AlternativeSystemFuelTypeWeightingValue value) : base()
        {
            Code = code;
            AlternativeSystemFuelTypeWeightingValue = value;
        }
    }
}
