using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Weightings for heat pump technology types
    /// </summary>
    public class HeatPumpTechnologyTypeWeighting : Entity
    {
        /// <summary>
        /// Weighting value for the given heat pump technology type
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Heat pump technology type code
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

        public HeatPumpTechnologyTypeWeighting(string code, decimal value) : base()
        {
            Code = code;
            Value = value;
        }
        protected HeatPumpTechnologyTypeWeighting() : base()
        { }
    }
}
